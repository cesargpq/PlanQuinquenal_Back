using AutoMapper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using iTextSharp.text.pdf.security;
using static iTextSharp.text.pdf.AcroFields;
using ApiDavis.Core.Utilidades;
using PlanQuinquenal.Core.DTOs;
using Microsoft.EntityFrameworkCore.Storage;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class InforemesActasRepository : IInforemesActasRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly HashService hashService;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public InforemesActasRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration, HashService hashService, ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.hashService = hashService;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }
        public async Task<ResponseDTO> Crear(InformeReqDTO informeReqDTO, DatosUsuario usuario)
        {

            try
            {

                var tipoInforme = await _context.TipoInforme.Where(x => x.Id == informeReqDTO.Tipo).FirstOrDefaultAsync();
                var tipoSeg = await _context.TipoSeguimiento.Where(x => x.Id == informeReqDTO.Modulo).FirstOrDefaultAsync();    
                bool existe;
                string tipoSeguimiento = "";
                if (tipoSeg.Descripcion.ToUpper().Equals("Proyectos".ToUpper()))
                {
                    var existePry = await _context.Proyecto.Where(x=>x.CodigoProyecto.Equals(informeReqDTO.CodigoProyecto) ).AnyAsync();
                    if (!existePry)
                    {
                        return new ResponseDTO
                        {
                            Valid = false,
                            Message = "No existe el proyecto ingresado"
                        };
                    }
                    else
                    {
                        tipoSeguimiento = "Proyectos";
                    }
                }
                else if (tipoSeg.Descripcion.ToUpper().Equals("Plan Quinquenal".ToUpper()))
                {
                    var existeQnq= await _context.PQuinquenal.Where(x => x.AnioPlan.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el plan quinquenal ingresado"
                        };
                    }
                    tipoSeguimiento = "Plan Quinquenal";

                }
                else if (tipoSeg.Descripcion.ToUpper().Equals("Plan Anual".ToUpper()))
                {
                    var existeQnq = await _context.PlanAnual.Where(x => x.AnioPlan.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el plan anual ingresado"
                        };
                    }
                    tipoSeguimiento = "Plan Anual";

                }
                else if (tipoSeg.Descripcion.ToUpper().Equals("Gestión de Reemplazo".ToUpper()))
                {
                    var existeQnq = await _context.BolsaReemplazo.Where(x => x.CodigoProyecto.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el Reemplazo ingresado"
                        };
                    }
                    tipoSeguimiento = "Gestión de Reemplazo";

                }




                var guidId = Guid.NewGuid();
                
                var informe = mapper.Map<Informe>(informeReqDTO);
                
                informe.TipoSeguimiento = tipoSeguimiento;
                informe.UsuarioRegister = usuario.UsuaroId;
                informe.FechaReunion = informeReqDTO.FechaReunion != null ? DateTime.Parse(informeReqDTO.FechaReunion) : null;
                informe.FechaInforme = informeReqDTO.FechaInforme != null ? DateTime.Parse(informeReqDTO.FechaInforme) : null;
                informe.FechaCompromiso = informeReqDTO.FechaCompromiso != null ? DateTime.Parse(informeReqDTO.FechaCompromiso) : null;
                informe.UsuarioModifica = usuario.UsuaroId;
                informe.FechaCreacion = DateTime.Now;
                informe.FechaModificacion = DateTime.Now;
                informe.Tipo = tipoInforme.Descripcion;
                informe.TipoInformeId = tipoInforme.Id;
                informe.Activo = true;

                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Actas , Crear").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Actas";
                    trazabilidad.Evento = "Crear";
                    trazabilidad.DescripcionEvento = $"Se creó correctamente el Acta o Informe {informe.Id} del proyecto {informe.CodigoProyecto} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }

               





                _context.Add(informe);
                await _context.SaveChangesAsync();



                #region Comparacion de estructuras y agregacion de cambios

                List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                CorreoTabla correoDatos = new CorreoTabla
                {
                    codigo = informe.CodigoProyecto.ToString()
                };

                composCorreo.Add(correoDatos);
                #endregion

                #region Envio de notificacion
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                var py = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(informe.CodigoProyecto)).FirstOrDefaultAsync();
                var usuInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == py.Id).ToListAsync();
                foreach (var listaUsuInters in usuInt)
                {
                    int cod_usu = listaUsuInters.UsuarioId;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regInfActas == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = "PROYECTOS";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = informe.CodigoProyecto;
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "C";
                        notifProyecto.mensaje = $"Se creó el {informe.Tipo} del proyecto {informe.CodigoProyecto}";
                        notifProyecto.codigo = py.Id;
                        notifProyecto.modulo = "I";

                        await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                        await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "I");
                    }
                }

                #endregion

                string ruta = "";
                string rutaFisica = "";
                string rutadirectory = "";
                string tipoDocumentoEnum = "";

                tipoDocumentoEnum = tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper() ? "I" : "A";
                informe.CodigoExpediente = tipoDocumentoEnum + informe.Id.ToString("D7");

                string tipo = tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper() ? "Informe" : "Acta";
                if (tipoSeg.Descripcion.Equals("Proyectos"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto +"\\"+ tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto  + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Proyectos" + "/" + informeReqDTO.CodigoProyecto +"/"+ tipo + "/" + guidId + ".pdf";
                }
                else if (tipoSeg.Descripcion.Equals("Plan Anual"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto +"\\"+ tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto  + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "PlanAnual" + "/" + informeReqDTO.CodigoProyecto +"/"+ tipo + "/" + guidId + ".pdf";
                }
                else if (tipoSeg.Descripcion.Equals("Plan Quinquenal"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Quinquenal" + "/" + informeReqDTO.CodigoProyecto + "/"+ tipo + "/" + guidId + ".pdf";

                }
                else if (tipoSeg.Descripcion.ToUpper().Equals("Gestión de Reemplazo".ToUpper()))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Gestión de Reemplazo\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Gestión de Reemplazo\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Gestión de Reemplazo" + "/" + informeReqDTO.CodigoProyecto + "/" + tipo + "/" + guidId + ".pdf";

                }
                informe.Ruta = ruta;
                informe.RutaFisica = rutaFisica;
                _context.Update(informe);
                await _context.SaveChangesAsync();





                if (tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper())
                {
                    List<string> listadeInteresados = new List<string>();
                    List<UsuariosInteresadosInformes> listaUint = new List<UsuariosInteresadosInformes>();
                    if (informeReqDTO.UserInteresados.Count > 0)
                    {
                        foreach (var user in informeReqDTO.UserInteresados)
                        {
                            var userIntInforme = new UsuariosInteresadosInformes
                            {
                                InformeId = informe.Id,
                                UsuarioId = user,
                                Activo = true
                            };
                            listaUint.Add(userIntInforme);
                            
                        }
                        _context.AddRange(listaUint);
                        await _context.SaveChangesAsync();
                        foreach (var item in informeReqDTO.UserInteresados)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listadeInteresados.Add(users.nombre_usu + " " + users.apellido_usu);
                        }

                    }
                    await CrearInformePdf(informe, rutadirectory, listadeInteresados);
                }
                else if (tipoInforme.Descripcion.ToUpper() == "Acta".ToUpper())
                {
                    List<string> listaParticipantes = new List<string>();
                    List<string> listaAsistentes = new List<string>();
                    List<ActaParticipantes> listaActaParti = new List<ActaParticipantes>();
                    List<ActaAsistentes> listaActaAsis = new List<ActaAsistentes>();
                    if (informeReqDTO.UserParticipantes.Count > 0)
                    {
                        foreach (var user in informeReqDTO.UserParticipantes)
                        {
                            var userIntInforme = new ActaParticipantes
                            {
                                InformeId = informe.Id,
                                UsuarioId = user,
                                Activo = false
                            };
                            listaActaParti.Add(userIntInforme);
                        }
                        _context.AddRange(listaActaParti);
                        await _context.SaveChangesAsync();
                        foreach (var item in informeReqDTO.UserParticipantes)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listaParticipantes.Add(users.nombre_usu + " " + users.apellido_usu);
                        }
                    }
                    if(informeReqDTO.UserAsistentes.Count > 0)
                    {
                        foreach (var user in informeReqDTO.UserAsistentes)
                        {
                            var userIntInforme = new ActaAsistentes
                            {
                                InformeId = informe.Id,
                                UsuarioId = user,
                                Activo = true
                            };
                            listaActaAsis.Add(userIntInforme);
                            
                        }
                        _context.AddRange(listaActaAsis);
                        await _context.SaveChangesAsync();
                        foreach (var item in informeReqDTO.UserAsistentes)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listaAsistentes.Add(users.nombre_usu + " " + users.apellido_usu);
                        }
                    }
                    string responsable = "";
                    if(informeReqDTO.Responsable!= 0)
                    {
                        var users = await _context.Usuario.Where(x => x.cod_usu == informeReqDTO.Responsable).FirstOrDefaultAsync();
                        responsable = users.nombre_usu + " " + users.apellido_usu;
                    }
                    List<ActaDinamica> acta = new List<ActaDinamica>();
                    if(informeReqDTO.ActaDinamicaRequest.Count() > 0)
                    {
                        foreach (var item in informeReqDTO.ActaDinamicaRequest)
                        {
                            var userIntActDin = new ActaDinamica
                            {
                                InformeId = informe.Id,
                                titulo = item.titulo,
                                descripcion = item.descripcion
                            };
                            acta.Add(userIntActDin);
                        }
                        _context.AddRange(acta);
                        await _context.SaveChangesAsync();
                    }
                    await CrearActa(informe, rutadirectory,listaParticipantes, listaAsistentes, responsable, informeReqDTO.ActaDinamicaRequest);
                }
                
               
                    
               

                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
            }
            catch (Exception e )
            {
                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };

            }
        }
        public async Task<bool> CrearActa(Informe informe,string rutaDirectory, List<string> listaParticipantes, List<string> listaAsistentes, string responsable, List<ActaDinamicaRequest> acta)
        {
            if (!Directory.Exists(rutaDirectory))
            {
                Directory.CreateDirectory(rutaDirectory);
            }
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER);

            PdfWriter writer = PdfWriter.GetInstance(doc,
                            new FileStream(informe.RutaFisica, FileMode.Create));

            doc.AddTitle("Calidda");
            doc.AddCreator("Proyecto Quinquenal");
            doc.Open();

            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font negrita = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK);


            hashService.Titulo(doc);
            hashService.Parrafo(doc, "ACTA DE REUNIÓN "+informe.CodigoExpediente, negrita, true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Fecha: {informe.FechaReunion?.ToString("dd/MM/yyyy")}", _standardFont, false);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, "Participantes:", _standardFont, false);
            hashService.GenerarLista(listaParticipantes, doc);

            hashService.Parrafo(doc, "Asistencia:", _standardFont, false);
            hashService.GenerarLista(listaAsistentes, doc);


            hashService.Parrafo(doc, $"Objetivo de la Reunión", _standardFont, false);
            hashService.ParrafoJustified(doc, informe.Objetivos, _standardFont, true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Agenda", _standardFont, false);
            hashService.ParrafoJustified(doc, informe.Agenda, _standardFont, true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Acuerdos:", _standardFont, false);

            if(informe.Acuerdos != null)
            {
                List<string> acuerdos = new List<string>();
                foreach (var item in informe.Acuerdos.Split("\n"))
                {
                    acuerdos.Add(item);
                }
                string cadenaAcuerdos = "";
                foreach (var item in acuerdos)
                {
                    cadenaAcuerdos += item + " ";
                }
                hashService.ParrafoJustified(doc, cadenaAcuerdos, _standardFont, true);
                doc.Add(Chunk.NEWLINE);
            }
            else
            {
                hashService.Parrafo(doc, "", _standardFont, false);
                doc.Add(Chunk.NEWLINE);
            }



            hashService.Parrafo(doc, $"Compromisos:", _standardFont, false);

            if(informe.Compromisos != null)
            {
                List<string> compromisos = new List<string>();
                foreach (var item in informe.Compromisos.Split("\n"))
                {
                    compromisos.Add(item);
                }

                hashService.GenerarLista(compromisos, doc);
            }
            else
            {
                List<string> compromisos = new List<string>();

                compromisos.Add("");


                hashService.GenerarLista(compromisos, doc);
            }
           


            hashService.Parrafo(doc, $"Responsable: {responsable}", _standardFont, false);


            foreach (var item in acta)
            {
                hashService.Parrafo(doc, $"{item.titulo}:", _standardFont, false);

                List<string> acuerdosActa = new List<string>();
                foreach (var items in item.descripcion.Split("\n"))
                {
                    acuerdosActa.Add(items);
                }

                hashService.GenerarLista(acuerdosActa, doc);
            }

            doc.Close();
            writer.Close();
            return true;
        }
        public async  Task<bool> CrearInformePdf(Informe informe,string rutadirectory, List<string> listadeInteresados)
        {
            if (!Directory.Exists(rutadirectory))
            {
                Directory.CreateDirectory(rutadirectory);
            }
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.LETTER);
          
            PdfWriter writer = PdfWriter.GetInstance(doc,
                            new FileStream(informe.RutaFisica, FileMode.Create));

            doc.AddTitle("Mi primer PDF");
            doc.AddCreator("Roberto Torres");
            doc.Open();
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font negrita = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE, BaseColor.BLACK);
            
            
            hashService.Titulo(doc);
            hashService.Parrafo(doc,"INFORME "+informe.CodigoExpediente,negrita,true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Fecha: {informe.FechaInforme?.ToString("dd/MM/yyyy")}", _standardFont, false);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Resumen general", _standardFont, false);
            hashService.ParrafoJustified(doc, informe.ResumenGeneral, _standardFont, true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc,$"Actividades a realizar:",_standardFont, false);
            hashService.ParrafoJustified(doc,informe.ActividadesRealizadas, _standardFont, true);
            doc.Add(Chunk.NEWLINE);
            hashService.Parrafo(doc, $"Próximos pasos:", _standardFont, false);

            List<string> proximosPasos = new List<string>();
            foreach (var item in informe.ProximosPasos.Split("\n"))
            {
                proximosPasos.Add(item);
            }

            hashService.GenerarLista(proximosPasos, doc);
            hashService.Parrafo(doc, "Interesados:", _standardFont, false);
            hashService.GenerarLista(listadeInteresados, doc);
            doc.Close();
            writer.Close();
            return true;
        }

        public async Task<ResponseEntidadDto<InformeResponseDto>> GetById(int id, DatosUsuario usuario)
        {
            try
            {
                var Informe =await _context.Informe.Where(x => x.Id == id)
                    .Include(x=>x.UserInteresados)
                    .ThenInclude(y=>y.Usuario)
                    .Include(x => x.Participantes)
                    .ThenInclude(y => y.Usuario)
                    .Include(x => x.Asistentes)
                    .ThenInclude(y => y.Usuario)
                    .Include(x=>x.ActaDinamica)
                   
                    .FirstOrDefaultAsync();


                var responseInforme = mapper.Map<InformeResponseDto>(Informe);






                    var tipoSeguimiento = await _context.TipoSeguimiento.Where(x => x.Descripcion.Equals(responseInforme.TipoSeguimiento)).FirstOrDefaultAsync();

                    responseInforme.TipoSeguimientoId = tipoSeguimiento.Id;

                    foreach (var item in responseInforme.Participantes)
                    {
                        var parti = await _context.ActaParticipantes.Where(x => x.InformeId == responseInforme.Id && x.UsuarioId == item.Usuario.Id).FirstOrDefaultAsync();

                        item.Usuario.Activo = parti.Activo;
                    }
                
                    var dato = await _context.ActaParticipantes.Where(x => x.InformeId == responseInforme.Id && x.UsuarioId == usuario.UsuaroId).FirstOrDefaultAsync();
                    
                    if (dato != null)
                    {
                        if(dato.Activo == true)
                        {
                            responseInforme.actaEstadoUser = true;
                            
                        }
                        else
                        {
                            responseInforme.actaEstadoUser = false;
                        }
                    }
                    else
                    {
                        responseInforme.actaEstadoUser = true;
                    }

                

                if (Informe != null)
                {
                    var response = new ResponseEntidadDto<InformeResponseDto>
                    {
                        Valid = true,
                        Message = Constantes.BusquedaExitosa,
                        Model = responseInforme
                    };
                    return response;
                }
                else
                {
                    var response = new ResponseEntidadDto<InformeResponseDto>
                    {
                        Valid = false,
                        Message = Constantes.BusquedaNoExitosa,
                        Model = null
                    };
                    return response;
                }
                
            }
            catch (Exception e)
            {

                var response = new ResponseEntidadDto<InformeResponseDto>
                {
                    Valid = false,
                    Message = Constantes.BusquedaNoExitosa,
                    Model = null
                };
                return response;
            }
        }

        public async Task<PaginacionResponseDto<InformeResponseDto>> GetAll(PaginationFilterActaDto pag)
        {

            var tipoInforme = await _context.TipoInforme.Where(x => x.Id == pag.TipoDocumento).FirstOrDefaultAsync();
            var tipoSeg = await _context.TipoSeguimiento.Where(x => x.Id == pag.TipoSeguimiento).FirstOrDefaultAsync();
            var queryable = _context.Informe
                    .OrderByDescending(x=>x.FechaCreacion)
                    .Where(x => pag.CodigoDocumento != "" ? x.CodigoExpediente.Contains(pag.CodigoDocumento) : true)
                    .Where(x => pag.TipoDocumento != 0 ? x.Tipo.Contains(tipoInforme.Descripcion) : true)
                    .Where(x => pag.TipoSeguimiento != 0 ? x.TipoSeguimiento.Contains(tipoSeg.Descripcion) : true)
                    .Where(x=> pag.CodigoProyecto != "" ? x.CodigoProyecto==pag.CodigoProyecto:true)
                    .Where(x => pag.Etapa != 0 ? x.Etapa == pag.Etapa : true)
                    .Where(x=>x.Activo==true)
                    .Include(x => x.UserInteresados)
                    .ThenInclude(y => y.Usuario)
                    .Include(x => x.Participantes)
                    .ThenInclude(y => y.Usuario)
                    .Include(x => x.Asistentes)
                    .ThenInclude(y => y.Usuario)
                    .AsQueryable();
            int cantidad = queryable.Count();
            var listaPaginada = await queryable.OrderByDescending(e => e.FechaCreacion).Paginar(pag).ToListAsync();

            
            var proyectoDto = mapper.Map<List<InformeResponseDto>>(listaPaginada);
            var informesPArt = await _context.ActaParticipantes.ToListAsync();
            foreach (var item in proyectoDto)
            {
                
                 var dato = informesPArt.Where(x => x.InformeId == item.Id).ToList();

                foreach (var part in item.Participantes)
                {
                   var usu = dato.Where(x => x.UsuarioId == part.Usuario.Id).FirstOrDefault();
                    part.Usuario.Activo = usu.Activo;
                }
                
            }

            foreach (var item in proyectoDto)
            {
                //item.actaEstado = proyectoDto. participante => participante.Participantes.es)
                if (item.Tipo.Equals("Informe"))
                {
                    item.actaEstado =null;
                }
                else
                {
                    var dato = item.Participantes.Any(x => x.Usuario.Activo == false);
                    if (dato)
                    {
                        item.actaEstado = false;
                    }
                    else
                    {
                        item.actaEstado = true;
                    }
                }
                
                
            }

            var pagination = new PaginacionResponseDto<InformeResponseDto>
            {
                Model = proyectoDto,
                Cantidad = cantidad,
            };
            return pagination;
        }


        public async Task<ResponseDTO> Update(InformeReqDTO informeReqDTO, int id, DatosUsuario usuario)
        {

            try
            {

                var tipoInforme = await _context.TipoInforme.Where(x => x.Id == informeReqDTO.Tipo).FirstOrDefaultAsync();
                var tipoSeg = await _context.TipoSeguimiento.Where(x => x.Id == informeReqDTO.Modulo).FirstOrDefaultAsync();
                bool existe;
                string tipoSeguimiento = "";
                if (tipoSeg.Descripcion.Equals("Proyectos"))
                {
                    var existePry = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(informeReqDTO.CodigoProyecto) ).AnyAsync();
                    if (!existePry)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el proyecto ingresado"
                        };
                    }
                    else
                    {
                        tipoSeguimiento = "Proyectos";
                    }
                }
                else if (tipoSeg.Descripcion.Equals("Plan Quinquenal"))
                {
                    var existeQnq = await _context.PQuinquenal.Where(x => x.AnioPlan.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el plan quinquenal ingresado"
                        };
                    }
                    tipoSeguimiento = "Plan Quinquenal";

                }
                else if (tipoSeg.Descripcion.Equals("Plan Anual"))
                {
                    var existeQnq = await _context.PlanAnual.Where(x => x.AnioPlan.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el plan anual ingresado"
                        };
                    }
                    tipoSeguimiento = "Plan Anual";

                }
                else if (tipoSeg.Descripcion.Equals("Reemplazo"))
                {
                    var existeQnq = await _context.BolsaReemplazo.Where(x => x.CodigoProyecto.Equals(informeReqDTO.CodigoProyecto)).AnyAsync();
                    if (!existeQnq)
                    {
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = "No existe el reemplazo ingresado"
                        };
                    }
                    tipoSeguimiento = "Reemplazo";

                }
                var guidId = Guid.NewGuid();
                var getInforme = await _context.Informe.Where(x => x.Id == id).FirstOrDefaultAsync();


                getInforme.ActividadesRealizadas = informeReqDTO.ActividadesRealizadas;
                getInforme.ProximosPasos = informeReqDTO.ProximosPasos;
                getInforme.ResumenGeneral = informeReqDTO.ResumenGeneral;
                getInforme.FechaReunion = informeReqDTO.FechaReunion != null ? DateTime.Parse(informeReqDTO.FechaReunion) : null;
                getInforme.FechaInforme = informeReqDTO.FechaInforme != null ? DateTime.Parse(informeReqDTO.FechaInforme) : null;
                getInforme.FechaCompromiso = informeReqDTO.FechaCompromiso != null ? DateTime.Parse(informeReqDTO.FechaCompromiso) : null;
                getInforme.UsuarioModifica = usuario.UsuaroId;
                getInforme.FechaModificacion = DateTime.Now;
                getInforme.Tipo = tipoInforme.Descripcion;
                getInforme.TipoInformeId = tipoInforme.Id;

                //Acta
                getInforme.Agenda = informeReqDTO.Agenda != null ? informeReqDTO.Agenda : null;
                getInforme.Objetivos = informeReqDTO.Objetivos != null ? informeReqDTO.Objetivos : null;
                getInforme.Acuerdos = informeReqDTO.Acuerdos != null ? informeReqDTO.Acuerdos : null;
                getInforme.Compromisos = informeReqDTO.Compromisos != null ? informeReqDTO.Compromisos : null;
                getInforme.Responsable = informeReqDTO.Responsable != null ? informeReqDTO.Responsable : null;
                _context.Update(getInforme);
                await _context.SaveChangesAsync();


                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Actas , Editar").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Actas";
                    trazabilidad.Evento = "Editar";
                    trazabilidad.DescripcionEvento = $"Se editó correctamente acta {getInforme.Id} del proyecto {getInforme.CodigoProyecto}";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }


                string ruta = "";
                string rutaFisica = "";
                string rutadirectory = "";
                string tipoDocumentoEnum = "";
                tipoDocumentoEnum = tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper() ? "I" : "A";
                getInforme.CodigoExpediente = tipoDocumentoEnum + getInforme.Id.ToString("D7");

                string tipo = tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper() ? "Informe" : "Acta";
                if (tipoSeg.Descripcion.Equals("Proyectos"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto  + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Proyectos" + "/" + informeReqDTO.CodigoProyecto + "/" + tipo + "/" + guidId + ".pdf";
                }
                else if (tipoSeg.Descripcion.Equals("Plan Anual"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "PlanAnual" + "/" + informeReqDTO.CodigoProyecto + "/" + tipo + "/" + guidId + ".pdf";
                }
                else if (tipoSeg.Descripcion.Equals("Plan Quinquenal"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Quinquenal" + "/" + informeReqDTO.CodigoProyecto + "/" + tipo + "/" + guidId + ".pdf";

                }
                else if (tipoSeg.Descripcion.Equals("Reemplazo"))
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Reemplazo\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Reemplazo\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Reemplazo" + "/" + informeReqDTO.CodigoProyecto + "/" + tipo + "/" + guidId + ".pdf";
                }
                getInforme.Ruta = ruta;
                getInforme.RutaFisica = rutaFisica;
                _context.Update(getInforme);
                await _context.SaveChangesAsync();





                if (tipoInforme.Descripcion.ToUpper() == "Informe".ToUpper())
                {
                    List<string> listadeInteresados = new List<string>();
                    List<UsuariosInteresadosInformes> listaUint = new List<UsuariosInteresadosInformes>();
                    if (informeReqDTO.UserInteresados.Count > 0)
                    {
                        var usuarios = await _context.UsuariosInteresadosInformes.Where(x => x.InformeId == getInforme.Id).ToListAsync();

                        foreach (var item in usuarios)
                        {
                            _context.Remove(item);
                            await _context.SaveChangesAsync();

                        }
                        foreach (var user in informeReqDTO.UserInteresados)
                        {
                            var userIntInforme = new UsuariosInteresadosInformes
                            {
                                InformeId = getInforme.Id,
                                UsuarioId = user,
                                Activo = true
                            };
                            listaUint.Add(userIntInforme);

                        }
                        foreach (var item in listaUint)
                        {
                            _context.Add(item);
                            await _context.SaveChangesAsync();

                        }
                        
                        foreach (var item in informeReqDTO.UserInteresados)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listadeInteresados.Add(users.nombre_usu + " " + users.apellido_usu);
                        }

                    }
                    await CrearInformePdf(getInforme, rutadirectory, listadeInteresados);
                }
                else if (tipoInforme.Descripcion.ToUpper() == "Acta".ToUpper())
                {
                    List<string> listaParticipantes = new List<string>();
                    List<string> listaAsistentes = new List<string>();
                    List<ActaParticipantes> listaActaParti = new List<ActaParticipantes>();
                    List<ActaAsistentes> listaActaAsis = new List<ActaAsistentes>();
                    if (informeReqDTO.UserParticipantes.Count > 0)
                    {
                        var usuarios = await _context.ActaParticipantes.Where(x => x.InformeId == getInforme.Id).ToListAsync();

                        foreach (var item in usuarios)
                        {
                            _context.Remove(item);
                            await _context.SaveChangesAsync();

                        }
                        foreach (var user in informeReqDTO.UserParticipantes)
                        {
                            var userIntInforme = new ActaParticipantes
                            {
                                InformeId = getInforme.Id,
                                UsuarioId = user,
                                Activo = true
                            };
                            listaActaParti.Add(userIntInforme);
                        }
                        foreach (var item in listaActaParti)
                        {
                            _context.Add(item);
                            await _context.SaveChangesAsync();

                        }
                        
                        foreach (var item in informeReqDTO.UserParticipantes)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listaParticipantes.Add(users.nombre_usu + " " + users.apellido_usu);
                        }
                    }
                    if (informeReqDTO.UserAsistentes.Count > 0)
                    {
                        var usuarios = await _context.ActaAsistentes.Where(x => x.InformeId == getInforme.Id).ToListAsync();
                        foreach (var item in usuarios)
                        {
                            _context.Remove(item);
                            await _context.SaveChangesAsync();

                        }
                        foreach (var user in informeReqDTO.UserAsistentes)
                        {
                            var userIntInforme = new ActaAsistentes
                            {
                                InformeId = getInforme.Id,
                                UsuarioId = user,
                                Activo = true
                            };
                            listaActaAsis.Add(userIntInforme);

                        }
                        foreach (var item in listaActaAsis)
                        {
                            _context.AddRange(item);
                            await _context.SaveChangesAsync();

                        }
                       
                        foreach (var item in informeReqDTO.UserParticipantes)
                        {
                            var users = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            listaAsistentes.Add(users.nombre_usu + " " + users.apellido_usu);
                        }
                    }
                    string responsable = "";
                    if (informeReqDTO.Responsable != 0)
                    {
                        var users = await _context.Usuario.Where(x => x.cod_usu == informeReqDTO.Responsable).FirstOrDefaultAsync();
                        responsable = users.nombre_usu + " " + users.apellido_usu;
                    }
                    List<ActaDinamica> acta = new List<ActaDinamica>();
                    if (informeReqDTO.ActaDinamicaRequest.Count() > 0)
                    {
                        var actadin = await _context.ActaDinamica.Where(x => x.InformeId == getInforme.Id).ToListAsync();
                        foreach (var item in actadin)
                        {
                            _context.Remove(item);
                            await _context.SaveChangesAsync();

                        }
                        foreach (var item in informeReqDTO.ActaDinamicaRequest)
                        {
                            var userIntActDin = new ActaDinamica
                            {
                                InformeId = getInforme.Id,
                                titulo = item.titulo,
                                descripcion = item.descripcion
                            };
                            acta.Add(userIntActDin);
                        }
                        foreach (var item in acta)
                        {
                            _context.Add(item);
                            await _context.SaveChangesAsync();

                        }
                       
                    }
                    await CrearActa(getInforme, rutadirectory, listaParticipantes, listaAsistentes, responsable, informeReqDTO.ActaDinamicaRequest);
                }





                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
            }
            catch (Exception e)
            {
                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };

            }
        }

        public async Task<DocumentoResponseDto> Download(int id)
        {

            DocumentoResponseDto obj = new DocumentoResponseDto();
            
                var dato = await _context.Informe.Where(x => x.Id == id).FirstOrDefaultAsync();
                
                obj.ruta = dato.RutaFisica;
            
            return obj;
        }

        public async Task<ResponseDTO> AprobarActa(AprobarActaDto a, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.Informe.Where(x => x.Id == a.CodigoActa).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var actaParticipante = await _context.ActaParticipantes.Where(x => x.UsuarioId == usuario.UsuaroId && x.InformeId == a.CodigoActa).FirstOrDefaultAsync();
                    if(actaParticipante != null)
                    {
                        actaParticipante.Activo = true;
                        _context.Update(actaParticipante);
                        await _context.SaveChangesAsync();


                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Actas , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Actas";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el acta {resultado.Id} del proyecto {resultado.CodigoProyecto} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = Constantes.AprobarActa
                        };
                    }
                    else
                    {
                        return new ResponseDTO
                        {
                            Valid = false,
                            Message = "El usuario no es un participante para aprobar el acta"
                        };
                    }
                   
                   
                }
                else
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.BusquedaNoExitosa
                    };
                }
            }
            catch (Exception e)
            {

                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
            }
        }

        public async Task<ResponseDTO> Delete(int id,DatosUsuario usuario)
        {
            var infomre = await _context.Informe.Where(x => x.Id == id).FirstOrDefaultAsync();
            infomre.Activo = false;
            _context.Update(infomre);
            await _context.SaveChangesAsync();
            var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Actas , Eliminar").ToListAsync();
            if (resultad.Count > 0)
            {
                Trazabilidad trazabilidad = new Trazabilidad();
                List<Trazabilidad> listaT = new List<Trazabilidad>();
                trazabilidad.Tabla = "Actas";
                trazabilidad.Evento = "Eliminar";
                trazabilidad.DescripcionEvento = $"Se eliminó correctamente el acta {infomre.Id} del proyecto {infomre.CodigoProyecto}";
                trazabilidad.UsuarioId = usuario.UsuaroId;
                trazabilidad.DireccionIp = usuario.Ip;
                trazabilidad.FechaRegistro = DateTime.Now;

                listaT.Add(trazabilidad);
                await _trazabilidadRepository.Add(listaT);
            }
            return new ResponseDTO { Valid = true, Message = Constantes.EliminacionSatisfactoria };
        }
    }
}
