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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class InforemesActasRepository : IInforemesActasRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly HashService hashService;

        public InforemesActasRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration, HashService hashService)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.hashService = hashService;
        }
        public async Task<ResponseDTO> Crear(InformeReqDTO informeReqDTO, int id)
        {

            try
            {
                bool existe;
                string tipoSeguimiento = "";
                if (informeReqDTO.Modulo == "PY")
                {
                    var existePry = await _context.Proyecto.Where(x=>x.CodigoProyecto.Equals(informeReqDTO.CodigoProyecto) && x.Etapa == informeReqDTO.Etapa).AnyAsync();
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
                else if (informeReqDTO.Modulo == "PQ")
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
                else if (informeReqDTO.Modulo == "PA")
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
                var guidId = Guid.NewGuid();
                var informe = mapper.Map<Informe>(informeReqDTO);
                informe.TipoSeguimiento = tipoSeguimiento;
                informe.UsuarioRegister = id;
                informe.FechaReunion = informeReqDTO.FechaReunion != null ? DateTime.Parse(informeReqDTO.FechaReunion) : null;
                informe.FechaInforme = informeReqDTO.FechaInforme != null ? DateTime.Parse(informeReqDTO.FechaInforme) : null;
                informe.FechaCompromiso = informeReqDTO.FechaCompromiso != null ? DateTime.Parse(informeReqDTO.FechaCompromiso) : null;
                informe.UsuarioModifica = id;
                informe.FechaCreacion = DateTime.Now;
                informe.FechaModificacion = DateTime.Now;
                _context.Add(informe);
                await _context.SaveChangesAsync();


                string ruta = "";
                string rutaFisica = "";
                string rutadirectory = "";
                informe.CodigoExpediente = "N°" + informe.Id.ToString("D7");

                string tipo = informeReqDTO.Tipo.ToUpper() == "Informe".ToUpper() ? "Informe" : "Acta";
                if (informeReqDTO.Modulo == "PY")
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto +"_"+ informeReqDTO.Etapa+"\\"+ tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\" + informeReqDTO.CodigoProyecto + "_" + informeReqDTO.Etapa + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Proyectos" + "/" + informeReqDTO.CodigoProyecto + "_" + informeReqDTO.Etapa +"/"+ tipo + "/" + guidId + ".pdf";
                }
                else if (informeReqDTO.Modulo == "PA")
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto +"\\"+ tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "PlanAnual\\" + informeReqDTO.CodigoProyecto  + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "PlanAnual" + "/" + informeReqDTO.CodigoProyecto +"/"+ tipo + "/" + guidId + ".pdf";
                }
                else if (informeReqDTO.Modulo == "PQ")
                {
                    rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\" + guidId + ".pdf";
                    rutadirectory = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + informeReqDTO.CodigoProyecto + "\\" + tipo + "\\";
                    ruta = configuration["DNS"] + "Quinquenal" + "/" + informeReqDTO.CodigoProyecto + "/"+ tipo + "/" + guidId + ".pdf";

                }
                informe.Ruta = ruta;
                informe.RutaFisica = rutaFisica;
                _context.Update(informe);
                await _context.SaveChangesAsync();





                if (informeReqDTO.Tipo.ToUpper() == "Informe".ToUpper())
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
                else if (informeReqDTO.Tipo.ToUpper() == "Acta".ToUpper())
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
                                Activo = true
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
                        foreach (var item in informeReqDTO.UserParticipantes)
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
            hashService.Parrafo(doc, $"Fecha: {informe.FechaReunion?.ToString("dd/MM/yyyy")}", _standardFont, false);

            hashService.Parrafo(doc, "Participantes:", _standardFont, false);
            hashService.GenerarLista(listaParticipantes, doc);

            hashService.Parrafo(doc, "Asistencia:", _standardFont, false);
            hashService.GenerarLista(listaAsistentes, doc);


            hashService.Parrafo(doc, $"Objetivo de la Reunión", _standardFont, false);
            hashService.GenerarTextAreaLista(informe.Objetivos, doc);

            hashService.Parrafo(doc, $"Agenda", _standardFont, false);
            hashService.GenerarTextAreaLista(informe.Agenda, doc);


            hashService.Parrafo(doc, $"Acuerdos:", _standardFont, false);

            List<string> acuerdos = new List<string>();
            foreach (var item in informe.Acuerdos.Split("\n"))
            {
                acuerdos.Add(item);
            }

            hashService.GenerarLista(acuerdos, doc);



            hashService.Parrafo(doc, $"Compromisos:", _standardFont, false);

            List<string> compromisos = new List<string>();
            foreach (var item in informe.Compromisos.Split("\n"))
            {
                compromisos.Add(item);
            }

            hashService.GenerarLista(compromisos, doc);


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
            hashService.Parrafo(doc, $"Fecha: {informe.FechaInforme?.ToString("dd/MM/yyyy")}", _standardFont, false);
            hashService.Parrafo(doc, $"Resumen general", _standardFont, false);
            hashService.GenerarTextAreaLista(informe.ResumenGeneral, doc);
            hashService.Parrafo(doc,$"Actividades a realizar:",_standardFont, false);
            hashService.GenerarTextAreaLista(informe.ActividadesRealizadas, doc);
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

        public async Task<ResponseEntidadDto<Informe>> GetById(int id)
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
                    .FirstOrDefaultAsync();

                var responseInforme = mapper.Map<InformeResponseDto>(Informe);
                if (Informe != null)
                {
                    var response = new ResponseEntidadDto<Informe>
                    {
                        Valid = true,
                        Message = Constantes.BusquedaExitosa,
                        Model = Informe
                    };
                    return response;
                }
                else
                {
                    var response = new ResponseEntidadDto<Informe>
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

                var response = new ResponseEntidadDto<Informe>
                {
                    Valid = false,
                    Message = Constantes.BusquedaNoExitosa,
                    Model = null
                };
                return response;
            }
        }
    }
}
