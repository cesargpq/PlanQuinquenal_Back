using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using ApiDavis.Core.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanQuinquenal.Core.DTOs;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class DocumentosRepository: IDocumentosRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public DocumentosRepository(PlanQuinquenalContext context, IConfiguration configuration,IMapper mapper, ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones) 
        {
            this._context = context;
            this.configuration = configuration;
            this.mapper = mapper;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }

        public async Task<ResponseDTO> Add(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            ResponseDTO obj =new ResponseDTO();
            if (documentoRequestDto.Modulo.Equals("PQ"))
            {
                var data  =await QuinquenalAdd(documentoRequestDto,usuario);

                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }
            else if (documentoRequestDto.Modulo.Equals("PY"))
            {
                var data = await ProyectosAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    
                }
                
            }
            else if (documentoRequestDto.Modulo.Equals("PA"))
            {
                var data = await PlanAnualAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }
            else if (documentoRequestDto.Modulo.Equals("BR"))
            {
                var data = await BolsaReemplazoAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }

            
            return obj;
        }

        public async Task<bool> BolsaReemplazoAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.BolsaReemplazo.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosBR>(documentoRequestDto);
                    map.BolsaReemplazoId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Reemplazo\\" + resultado.CodigoProyecto + "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.ruta = configuration["DNS"] + "Reemplazo" + "/" + resultado.CodigoProyecto + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId,resultado.CodigoProyecto);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }

                    #region Notificacion
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = resultado.CodigoProyecto
                    };
                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    string asunto = $"Se creó el documento {documentoRequestDto.NombreDocumento} en la Bolsa de Reemplazo {resultado.CodigoProyecto}";
                    var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regImp == true).ToListAsync();
                        string correo = listaUsuInters.correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Bolsa de Reemplazo";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = resultado.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el documento {documentoRequestDto.NombreDocumento} en la Bolsa de Reemplazo {resultado.CodigoProyecto}";
                            notifProyecto.codigo = resultado.Id;
                            notifProyecto.modulo = "BR";
                            notificacionList.Add(notifProyecto);
                            correosList.Add(correo);
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Bolsa de Reemplazo", asunto);
                    #endregion
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PlanAnualAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.PlanAnual.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPA>(documentoRequestDto);
                    map.PlanAnualId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] +"\\"+"PlanAnual\\"+ resultado.AnioPlan+"\\"+guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"]+ "PlanAnual"+ "/" + resultado.AnioPlan + "/" + guidId+ Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId,resultado.AnioPlan);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }

                    #region Notificacion
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    string asunto = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el Plan Anual {resultado.AnioPlan}";
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = resultado.AnioPlan
                    };
                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPQ == true).ToListAsync();
                        string correo = listaUsuInters.correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Plan Anual";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = resultado.AnioPlan;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el Plan Anual {resultado.AnioPlan}";
                            notifProyecto.codigo = resultado.Id;
                            notifProyecto.modulo = "PA";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Plan Anual", asunto);
                    #endregion
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> QuinquenalAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                
                var resultado = await _context.PQuinquenal.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPQ>(documentoRequestDto);
                    map.PQuinquenalId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + resultado.AnioPlan + "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Quinquenal"+"/" + resultado.AnioPlan + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();


                    saveDocument(documentoRequestDto, guidId, resultado.AnioPlan);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }

                    #region Notificacion
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    string asunto = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el Plan Quinquenal {resultado.AnioPlan}";
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = resultado.AnioPlan
                    };
                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPQ == true).ToListAsync();
                        string correo = listaUsuInters.correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Plan Quinquenal";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = resultado.AnioPlan;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el Plan Quinquenal {resultado.AnioPlan}";
                            notifProyecto.codigo = resultado.Id;
                            notifProyecto.modulo = "PQ";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Plan Quinquenal", asunto);
                    #endregion

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> ProyectosAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(documentoRequestDto.CodigoProyecto)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPy>(documentoRequestDto);
                    map.CodigoProyecto = resultado.CodigoProyecto;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\"+ resultado.CodigoProyecto +   "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Proyectos" + "/" + resultado.CodigoProyecto+ "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();
                   
                    saveDocument(documentoRequestDto, guidId,resultado.CodigoProyecto);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }


                    #region Notificacion

                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    var usuariosInteresados = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto == documentoRequestDto.CodigoProyecto).ToListAsync();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = resultado.CodigoProyecto
                    };
                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    string asunto = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el proyecto {resultado.CodigoProyecto}";

                    foreach (var listaUsuInters in usuariosInteresados)
                    {
                        int cod_usu = listaUsuInters.UsuarioId;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPry == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "PROYECTOS";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = resultado.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el documento {documentoRequestDto.NombreDocumento} en el proyecto {resultado.CodigoProyecto}";
                            notifProyecto.codigo = resultado.Id;
                            notifProyecto.modulo = "P";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                            
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Proyectos", asunto);

                    #endregion
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception e)
            {
                return false;
            }
           
            
           
        }
        public bool saveDocument(DocumentoRequestDto documentoRequestDto, Guid guidId,string CodigoProyecto)
        {
            try
            {
                string rutaCompleta = "";
                string ruta = "";
                string modulo = "";
                if (documentoRequestDto.Modulo.Equals("PY"))
                {
                    modulo = "Proyectos";
                }
                else if (documentoRequestDto.Modulo.Equals("PQ"))
                {
                    modulo = "Quinquenal";
                }
                else if (documentoRequestDto.Modulo.Equals("PA"))
                {
                    modulo = "PlanAnual";
                }
                else if (documentoRequestDto.Modulo.Equals("BR"))
                {
                    modulo = "Reemplazo";
                }
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";
               
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

               
               
                    rutaCompleta = ruta + CodigoProyecto;
                
                    
                if (!Directory.Exists(rutaCompleta))
                {
                    Directory.CreateDirectory(rutaCompleta);
                }
                string rutaSave = Path.Combine(rutaCompleta, guidId+Path.GetExtension(documentoRequestDto.NombreDocumento));

                byte[] decodedBytes = Convert.FromBase64String(documentoRequestDto.base64);
                File.WriteAllBytes(rutaSave, decodedBytes);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public async Task<ResponseDTO> GetUrl(int id, string modulo)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {
                
                if (modulo.Equals("PA"))
                {
                    var data = await _context.DocumentosPA.Where(x => x.Id == id && x.Estado==true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                else if (modulo.Equals("PY"))
                {
                    var data = await _context.DocumentosPy.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                    
                }
                else if (modulo.Equals("PQ"))
                {
                    var data = await _context.DocumentosPQ.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                else if (modulo.Equals("BR"))
                {
                    var data = await _context.DocumentosBR.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                return obj;
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
            
           
        }
        public async Task<DocumentoResponseDto> Download(int id, string modulo)
        {

            DocumentoResponseDto obj = new DocumentoResponseDto();
            if (modulo.Equals("PA"))
            {
                var dato = await _context.DocumentosPA.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("PY"))
            {
                var dato = await _context.DocumentosPy.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("PQ"))
            {
                var dato = await _context.DocumentosPQ.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("BR"))
            {
                var dato = await _context.DocumentosBR.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutaFisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            return obj;
            
        }

        public async Task<ResponseDTO> Delete(int id, string modulo,DatosUsuario usuario)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {
                try
                {
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    
                    if (modulo.Equals("PA"))
                    {
                        
                        
                        var dato = await _context.DocumentosPA.Where(x => x.Id == id).FirstOrDefaultAsync();
                        var plananual = await _context.PlanAnual.Where(x => x.Id == dato.PlanAnualId).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;

                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el documento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }

                        List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                        CorreoTabla correoDatos = new CorreoTabla
                        {
                            codigo = plananual.AnioPlan
                        };
                        composCorreo.Add(correoDatos);
                        List<string> correosList = new List<string>();
                        List<Notificaciones> notificacionList = new List<Notificaciones>();
                        string asunto = $"Se eliminó el documento {dato.NombreDocumento} del plan anual {plananual.AnioPlan}";

                        var usuarioInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                        foreach (var listaUsuInters in usuarioInt)
                        {
                            int cod_usu = listaUsuInters.cod_usu;
                            var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPQ == true).ToListAsync();
                            string correo = listaUsuInters.correo_usu.ToString();
                            if (lstpermisos.Count() == 1)
                            {
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = cod_usu;
                                notifProyecto.seccion = "Plan Anual";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = plananual.AnioPlan;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "D";
                                notifProyecto.mensaje = $"Se eliminó el documento {dato.NombreDocumento} del plan anual {plananual.AnioPlan}";
                                notifProyecto.codigo = plananual.Id;
                                notifProyecto.modulo = "P";
                                correosList.Add(correo);
                                notificacionList.Add(notifProyecto);
                            }
                        }
                        await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                        await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "D", "Documentos", asunto);

                        return obj;

                    }
                    else if (modulo.Equals("PY"))
                    {
                        ;
                        
                        var dato = await _context.DocumentosPy.Where(x => x.Id == id).FirstOrDefaultAsync();
                        var proyecto = await _context.Proyecto.Where(x => x.CodigoProyecto == dato.CodigoProyecto).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el documento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }

                        List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                        CorreoTabla correoDatos = new CorreoTabla
                        {
                            codigo = proyecto.CodigoProyecto
                        };
                        composCorreo.Add(correoDatos);
                        List<string> correosList = new List<string>();
                        List<Notificaciones> notificacionList = new List<Notificaciones>();
                        string asunto = $"Se eliminó el documento {dato.NombreDocumento} del proyecto {proyecto.CodigoProyecto}";

                        var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto.Equals(proyecto.CodigoProyecto)).ToListAsync();
                        foreach (var listaUsuInters in usuarioInt)
                        {
                            int cod_usu = listaUsuInters.UsuarioId;
                            var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPry == true).ToListAsync();
                            var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                            string correo = UsuarioInt[0].correo_usu.ToString();
                            if (lstpermisos.Count() == 1)
                            {
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = cod_usu;
                                notifProyecto.seccion = "Proyecto";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = proyecto.CodigoProyecto;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "D";
                                notifProyecto.mensaje = $"Se eliminó el documento {dato.NombreDocumento} del proyecto {proyecto.CodigoProyecto}";
                                notifProyecto.codigo = proyecto.Id;
                                notifProyecto.modulo = "P";
                                correosList.Add(correo);
                                notificacionList.Add(notifProyecto);
                               
                            }
                        }
                        await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                        await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "D", "Documentos",asunto);

                        return obj;
                    }
                    else if (modulo.Equals("PQ"))
                    {

                        var dato = await _context.DocumentosPQ.Where(x => x.Id == id).FirstOrDefaultAsync();
                        var planQuinquenal = await _context.PQuinquenal.Where(x => x.Id == dato.PQuinquenalId).FirstOrDefaultAsync();

                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }

                        List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                        CorreoTabla correoDatos = new CorreoTabla
                        {
                            codigo = planQuinquenal.AnioPlan
                        };
                        composCorreo.Add(correoDatos);
                        List<string> correosList = new List<string>();
                        List<Notificaciones> notificacionList = new List<Notificaciones>();
                        string asunto = $"Se eliminó el documento {dato.NombreDocumento} del Plan Quinquenal {planQuinquenal.AnioPlan}";

                        var usuarisInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                        foreach (var listaUsuInters in usuarisInt)
                        {
                            int cod_usu = listaUsuInters.cod_usu;
                            var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPQ == true).ToListAsync();
                            string correo = listaUsuInters.correo_usu.ToString();
                            if (lstpermisos.Count() == 1)
                            {
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = cod_usu;
                                notifProyecto.seccion = "Plan Quinquenal";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = planQuinquenal.AnioPlan;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "C";
                                notifProyecto.mensaje = $"Se eliminó el documento {dato.NombreDocumento} del Plan Quinquenal {planQuinquenal.AnioPlan}";
                                notifProyecto.codigo = planQuinquenal.Id;
                                notifProyecto.modulo = "P";

                                correosList.Add(correo);
                                notificacionList.Add(notifProyecto);
                            }
                        }
                        await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                        await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "D", "Documentos", asunto);

                        return obj;
                    }
                    else if (modulo.Equals("BR"))
                    {
                        var dato = await _context.DocumentosBR.Where(x => x.Id == id).FirstOrDefaultAsync();
                        var bolsaReemplazo = await _context.BolsaReemplazo.Where(x => x.Id == dato.BolsaReemplazoId).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }

                        List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                        CorreoTabla correoDatos = new CorreoTabla
                        {
                            codigo = bolsaReemplazo.CodigoProyecto
                        };
                        composCorreo.Add(correoDatos);
                        List<string> correosList = new List<string>();
                        List<Notificaciones> notificacionList = new List<Notificaciones>();
                        string asunto = $"Se eliminó el documento {dato.NombreDocumento} de la Bolsa de Reemplazo {bolsaReemplazo.CodigoProyecto}";


                        var usuarisInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                        foreach (var listaUsuInters in usuarisInt)
                        {
                            int cod_usu = listaUsuInters.cod_usu;
                            var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regImp == true).ToListAsync();
                            var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                            string correo = UsuarioInt[0].correo_usu.ToString();
                            if (lstpermisos.Count() == 1)
                            {
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = cod_usu;
                                notifProyecto.seccion = "Plan Quinquenal";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = bolsaReemplazo.CodigoProyecto;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "C";
                                notifProyecto.mensaje = $"Se eliminó el documento {dato.NombreDocumento} de la Bolsa de Reemplazo {bolsaReemplazo.CodigoProyecto}";
                                notifProyecto.codigo = bolsaReemplazo.Id;
                                notifProyecto.modulo = "P";

                                correosList.Add(correo);
                                notificacionList.Add(notifProyecto);
                            }
                        }
                        await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                        await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "D", "Documentos", asunto);
                        return obj;
                    }

                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    return obj;
                }
                catch (Exception e)
                {

                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    return obj;
                }
               
               
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
            
            
        }

        public async Task<PaginacionResponseDto<DocumentoResponseDto>> Listar(ListDocumentosRqDTO listDocumentosRequestDto)
        {
            if (listDocumentosRequestDto.Modulo == "PY")
            {
                try
                {
                    var proyecto = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(listDocumentosRequestDto.CodigoProyecto) ).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPy
                                    .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                                    .Where(x=>x.CodigoProyecto == proyecto.CodigoProyecto)
                                    .Where(x=> x.Estado==true)
                                    .OrderBy(x=>x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto  >>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e )
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }
               
            }
            else if (listDocumentosRequestDto.Modulo == "PQ")
            {
                try
                {
                    var pq = await _context.PQuinquenal.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPQ
                            .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                            .Where(x => x.PQuinquenalId == pq.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();
                       

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }
            else if (listDocumentosRequestDto.Modulo == "PA")
            {
                try
                {
                    var pa = await _context.PlanAnual.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPA
                        .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                        .Where(x => x.PlanAnualId == pa.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }
            else if (listDocumentosRequestDto.Modulo == "BR")
            {
                try
                {
                    var br = await _context.BolsaReemplazo.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosBR
                        .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                        .Where(x => x.BolsaReemplazoId == br.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }

            throw new NotImplementedException();
        }
    }
}
