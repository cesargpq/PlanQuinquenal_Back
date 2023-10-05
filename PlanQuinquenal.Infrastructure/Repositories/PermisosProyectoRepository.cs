using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using ApiDavis.Core.Utilidades;
using PlanQuinquenal.Core.DTOs;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PermisosProyectoRepository : IPermisosProyectoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public PermisosProyectoRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration, ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones, IRepositoryMantenedores repositoryMantenedores)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryMantenedores = repositoryMantenedores;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }

        public async Task<ResponseDTO> Add(PermisoRequestDTO permisoRequestDTO, DatosUsuario usuario)
        {
            try
            {
                var existeProyecto = await _context.Proyecto.Where(x => x.Id == permisoRequestDTO.ProyectoId).FirstOrDefaultAsync();
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();

                if (existeProyecto != null)
                {
                    var obtenerPermiso = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(permisoRequestDTO.TipoPermisosProyecto.ToUpper())).FirstOrDefaultAsync();
                    var distrito = await _context.Distrito.Where(x => x.Id == existeProyecto.DistritoId).FirstOrDefaultAsync();
                    
                    if (obtenerPermiso == null)
                    {
                        return new ResponseDTO
                        {
                            Valid = false,
                            Message = "El permiso envíado no existe"
                        };
                    }
                    
                    PermisosProyecto obj = new PermisosProyecto();
                    obj.ProyectoId = permisoRequestDTO.ProyectoId;
                    obj.TipoPermisosProyectoId = obtenerPermiso.Id;
                    obj.DistritoId = distrito.Descripcion;
                    obj.Expediente = permisoRequestDTO.Expediente;
                    obj.Longitud = permisoRequestDTO.Longitud;
                    obj.EstadoPermisosId = permisoRequestDTO.EstadoPermisosId;
                    obj.Estado = true;
                    obj.FechaCreacion = DateTime.Now;
                    obj.FechaModificacion = DateTime.Now;
                    obj.UsuarioCreacion = usuario.UsuaroId;
                    obj.UsuarioModifca = usuario.UsuaroId;
                    _context.Add(obj);
                    await _context.SaveChangesAsync();

                   
                    Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Permisos";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se créo correctamente el permiso {permisoRequestDTO.TipoPermisosProyecto} del proyecto {existeProyecto.CodigoProyecto} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    

                    var resultado = await _context.PermisosProyecto.Where(x => x.ProyectoId == permisoRequestDTO.ProyectoId && x.Estado ==true).ToListAsync();

                    var porcenaje = await _context.Constante.Where(x => x.Descripcion.Equals("% desviación")).FirstOrDefaultAsync();

                    decimal? resultadoLong =resultado.Sum(objeto => objeto.Longitud);
                    var LongAprobada = ((Convert.ToDecimal(porcenaje.valor) / 100) * existeProyecto?.LongAprobPa) + existeProyecto.LongAprobPa;


                    var perfiles = await _context.Perfil.Where(x => x.nombre_perfil.Equals("Legal") || x.nombre_perfil.Equals("Administrador")).ToListAsync();
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    List<Usuario> usuLegal = new List<Usuario>();
                    if (perfiles != null)
                    {
                        foreach (var item in perfiles)
                        {
                            var uzu = await _context.Usuario.Where(x => x.estado_user == "A" && x.Perfilcod_perfil == item.cod_perfil).ToListAsync();
                            usuLegal.AddRange(uzu);
                        }
                        if (resultadoLong > LongAprobada)
                        {
                            foreach (var item in usuLegal)
                            {
                                int cod_usu = item.cod_usu;
                                var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPer == true).ToListAsync();
                                var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                                string correo = UsuarioInt[0].correo_usu.ToString();
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = cod_usu;
                                notifProyecto.seccion = $"Proyecto - Permiso";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = existeProyecto.CodigoProyecto;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "C";
                                notifProyecto.mensaje = $"La sumatoria de permisos ({String.Format("{0:#,##0.##}",Math.Round((decimal)resultadoLong,2))} m) en el Proyecto {existeProyecto.CodigoProyecto} es mayor a la Longitud Aprobada del proyecto {String.Format("{0:#,##0.##}",Math.Round((decimal)existeProyecto.LongAprobPa,2))} m";
                                notifProyecto.codigo = existeProyecto.Id;
                                notifProyecto.modulo = "P";
                                correosList.Add(correo);
                                notificacionList.Add(notifProyecto);
                            }
                        }
                    }
                    
                    #region Envio de notificacion

                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                       codigo = existeProyecto.CodigoProyecto
                    };
                   
                    string asunto = $"Se registró el Permiso {obtenerPermiso.Descripcion} Expediente {obj.Expediente} en Proyecto {existeProyecto.CodigoProyecto}";
                    composCorreo.Add(correoDatos);
                    var usuInt = await _context.Usuario.Where(x => x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPer == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = $"Proyecto - Permiso {obtenerPermiso.Descripcion}";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = existeProyecto.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se registró el Permiso {obtenerPermiso.Descripcion} Expediente {obj.Expediente} en Proyecto {existeProyecto.CodigoProyecto}";
                            notifProyecto.codigo = existeProyecto.Id;
                            notifProyecto.modulo = "P";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", $"Proyecto - Permiso {obtenerPermiso.Descripcion} Expediente {obj.Expediente}", asunto);
                    #endregion

                    return new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.CreacionExistosa
                    };
                }

                else
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = "El proyecto ingresado no existe"
                    };
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

        public async Task<List<CorreoTabla>> CompararPropiedadesAsync(string codigo, object valOriginal, string nomCompleto)
        {
            List<CorreoTabla> camposModificados = new List<CorreoTabla>();
            if (valOriginal!= null)
            {
                DateTime fechaActual = DateTime.Today;
                string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
                var compara = _context.Entry(valOriginal).Properties.Where(p=>p.IsModified).ToList();

                foreach (var item in compara)
                {
                    if (item.Metadata.Name.Equals("FechaModifica") || item.Metadata.Name.Equals("UsuarioModifica")  || item.Metadata.Name.Equals("FechaModificacion"))
                    {
                        continue;
                    }
                    else if (item.Metadata.Name.Equals("EstadoPermisosId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("EstadoPermisos");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Estado Permiso");
                        camposModificados.Add(fila);
                    }
                    else
                    {
                        CorreoTabla fila = CrearCorreoTablaDirecto(codigo, nomCompleto, fechaFormateada, item, SepararPalabrasConMayusculas(item.Metadata.Name));
                        camposModificados.Add(fila);
                    }
                }
            }

            return camposModificados;
        }

        private string SepararPalabrasConMayusculas(string input)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];
                if (char.IsUpper(currentChar) && i > 0)
                {
                    result.Append(' ');
                }
                result.Append(currentChar);
            }

            return result.ToString().Replace("ion","ión");
        }

        private static CorreoTabla CrearCorreoTablaManualFecha(string codigo,PropertyEntry? item, string nomCompleto, string fechaFormateada,string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = codigo,
                valorActual = item.OriginalValue!= null ? ((DateTime)item.OriginalValue).ToString("dd/MM/yyyy"): "",
                campoModificado = nombreColumna,
                valorModificado = item.CurrentValue!= null ?((DateTime)item.CurrentValue).ToString("dd/MM/yyyy"): "",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTablaManual(string codigo,string valorActual,string valorModificado, string nomCompleto, string fechaFormateada,string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = codigo,
                valorActual = valorActual,
                campoModificado = nombreColumna,
                valorModificado = valorModificado,
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTabla(string valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, IEnumerable<MaestroResponseDto> enumerable, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = valOriginal,
                valorActual = item.OriginalValue!= null ? enumerable.Where(x => x.Id == (int)item.OriginalValue).FirstOrDefault().Descripcion: "",
                campoModificado = nombreColumna,
                valorModificado =item.CurrentValue!=null? enumerable.Where(x => x.Id == (int)item.CurrentValue).FirstOrDefault().Descripcion: "",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTablaDirecto(string valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = valOriginal,
                valorActual =item.OriginalValue != null ? (item.OriginalValue is bool?( (bool)item.OriginalValue? "Marcado" : "Desmarcado"):  item.OriginalValue +"" ):"",
                campoModificado = nombreColumna,
                valorModificado = item.CurrentValue != null ? (item.CurrentValue is bool?( (bool)item.CurrentValue? "Marcado" : "Desmarcado"): item.CurrentValue +"" ):"",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }
        public async Task<PaginacionResponseDtoException<RequestPermisoGetDto>> ListarPermisos(PermisosListDto p)
        {
            var tipoPermiso = await _context.TipoPermisosProyecto.Where(x=>x.Descripcion.ToUpper().Equals(p.tipoPermiso.ToUpper())).FirstOrDefaultAsync();
            var obtenerPermiso = await _context.RequestPermisoGetDto.FromSqlInterpolated($"EXEC ObtenerPermisoList  {p.ProyectoId} , {tipoPermiso.Id} , {p.Pagina} , {p.RecordsPorPagina} ").ToListAsync();

            if (obtenerPermiso == null)
            {
                return new PaginacionResponseDtoException<RequestPermisoGetDto>
                {
                    Model = null,
                    Cantidad=0
                };
            }
            else
            {

                return new PaginacionResponseDtoException<RequestPermisoGetDto>
                {
                    Cantidad = obtenerPermiso.Count() == 0 ? 0 : obtenerPermiso.ElementAt(0).Total,
                    Model = obtenerPermiso
                };
            }
        }
        public async Task<ResponseEntidadDto<RequestPermisoGetDto>> GetPermiso(int ProyectoId)
        {
            //var tipoPerm = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(TipoPermiso.ToUpper())).FirstOrDefaultAsync();
            //if (tipoPerm == null)
            //{
            //    return new ResponseEntidadDto<RequestPermisoGetDto>
            //    {
            //        Model = null,
            //        Valid = true,
            //        Message = "No existe al tipo de permiso envíado"
            //    };
            //}
            //else
            //{
                var obtenerPermiso = await _context.RequestPermisoGetDto.FromSqlRaw($"EXEC ObtenerPermiso  {ProyectoId}").ToListAsync();
                if (obtenerPermiso == null)
                {
                    return new ResponseEntidadDto<RequestPermisoGetDto>
                    {
                        Model = null,
                        Valid = true,
                        Message = "No existe el permiso solicitado"
                    };
                }
                else
                {
                  
                    return new ResponseEntidadDto<RequestPermisoGetDto>
                    {
                        Model = obtenerPermiso.ElementAt(0),
                        Valid = true,
                        Message = Constantes.BusquedaExitosa
                    };
                }
            //}
            
        }

        public async Task<ResponseDTO> CargarExpediente(DocumentosPermisosRequestDTO documentosPermisosRequestDTO, DatosUsuario usuario)
        {
            var existeProyecto = await _context.PermisosProyecto.Where(x => x.Id == documentosPermisosRequestDTO.PermisoId).FirstOrDefaultAsync();

            if(existeProyecto != null)
            {
                var tipoPerm = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(documentosPermisosRequestDTO.TipoPermisosProyecto.ToUpper())).FirstOrDefaultAsync();
                var proyecto = await _context.Proyecto.Where(x => x.Id == existeProyecto.ProyectoId).FirstOrDefaultAsync();
                DocumentosPermisos documentos = new DocumentosPermisos();

                var guidId = Guid.NewGuid();
                var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                documentos.PermisoId = existeProyecto.Id;
                documentos.TipoPermisosProyectoId = tipoPerm.Id;
                documentos.NombreDocumento = documentosPermisosRequestDTO.NombreDocumento;
                documentos.CodigoDocumento = guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.Fecha = Convert.ToDateTime(documentosPermisosRequestDTO.Fecha);
                documentos.Expediente = existeProyecto.Expediente;
                documentos.rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" +"Proyectos\\"+ proyecto.CodigoProyecto + $"\\Permiso\\{tipoPerm.Descripcion}\\" + guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.ruta = configuration["DNS"] + "Proyectos" + "/" + proyecto.CodigoProyecto  + $"/Permiso/{tipoPerm.Descripcion}/" + guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.FechaCreacion = DateTime.Now;
                documentos.FechaModificacion = DateTime.Now;
                documentos.UsuarioCreacion = usuario.UsuaroId;
                documentos.UsuarioModifca = usuario.UsuaroId;
                documentos.Estado = true;
                documentos.Vencimiento = documentosPermisosRequestDTO.Vencimiento!="" || documentosPermisosRequestDTO.Vencimiento != null ? DateTime.Parse(documentosPermisosRequestDTO.Vencimiento):null;
                _context.Add(documentos);
                


                
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Permisos";
                    trazabilidad.Evento = "CargarExpediente";
                    trazabilidad.DescripcionEvento = $"Se cargó correctamente el expediente {documentos.Expediente} en el proyecto {proyecto.CodigoProyecto}";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
               
                saveDocument(documentosPermisosRequestDTO, guidId, tipoPerm.Descripcion, proyecto.CodigoProyecto);

                #region Envio de notificacion
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                List<string> correosList = new List<string>();
                List<Notificaciones> notificacionList = new List<Notificaciones>();
                List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                CorreoTabla correoDatos = new CorreoTabla
                {
                    codigo = proyecto.CodigoProyecto
                };
                
                string asunto = $"Se registró un Documento {documentos.NombreDocumento} en el Permiso {tipoPerm.Descripcion} Expediente {existeProyecto.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                composCorreo.Add(correoDatos);
                var usuInt = await _context.Usuario.Where(x => x.estado_user == "A").ToListAsync();
                foreach (var listaUsuInters in usuInt)
                {
                    int cod_usu = listaUsuInters.cod_usu;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPer == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = $"Proyecto - Permiso {tipoPerm.Descripcion}";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = proyecto.CodigoProyecto;
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "C";
                        notifProyecto.mensaje = asunto;
                        notifProyecto.codigo = existeProyecto.Id;
                        notifProyecto.modulo = "P";
                        correosList.Add(correo);
                        notificacionList.Add(notifProyecto);
                    }
                }
                await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", $"Proyecto - Permiso {tipoPerm.Descripcion} Expediente {existeProyecto.Expediente} Documento {documentos.NombreDocumento}", asunto);
                #endregion
                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
            }
            else
            {

                return new ResponseDTO
                {
                    Valid = false,
                    Message = "No existe el proyecto y etapa solicitado"
                };

            }
            
        }
         public bool saveDocument(DocumentosPermisosRequestDTO documentoRequestDto, Guid guidId,string tipopermiso, string CodigoProyecto)
        {
            try
            {
                string rutaCompleta = "";
                string ruta = "";
                string modulo = "Proyectos";
               
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";
               
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                
                rutaCompleta = ruta + CodigoProyecto+ $"\\Permiso\\{tipopermiso}\\";
                                    
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

        public async Task<ResponseDTO> Delete(int id,DatosUsuario usuario)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {
                var dato = await _context.DocumentosPermisos.Where(x => x.Id == id).FirstOrDefaultAsync();
                var existeProyecto = await _context.PermisosProyecto.Where(x => x.Id == dato.PermisoId).FirstOrDefaultAsync();
                var tipoPerm = await _context.TipoPermisosProyecto.Where(x => x.Id == existeProyecto.TipoPermisosProyectoId).FirstOrDefaultAsync();
                var proyecto = await _context.Proyecto.Where(x => x.Id == existeProyecto.ProyectoId).FirstOrDefaultAsync();
                
                dato.Estado = false;
                dato.UsuarioModifca = usuario.UsuaroId;
                dato.FechaModificacion = DateTime.Now;
                _context.Update(dato);
                await _context.SaveChangesAsync();


                
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Permisos";
                    trazabilidad.Evento = "Eliminar";
                    trazabilidad.DescripcionEvento = $"Se elimnó correctamente el expediente  {dato.Expediente} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                

                #region Envio de notificacion

                List<string> correosList = new List<string>();
                List<Notificaciones> notificacionList = new List<Notificaciones>();
                List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                CorreoTabla correoDatos = new CorreoTabla
                {
                    codigo = dato.Id+""
                };
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string asunto = $"Se eliminó Documento {dato.NombreDocumento} en el Permiso {tipoPerm.Descripcion} Expediente {existeProyecto.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                composCorreo.Add(correoDatos);
                var usuInt = await _context.Usuario.Where(x => x.estado_user == "A").ToListAsync();
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                foreach (var listaUsuInters in usuInt)
                {
                    int cod_usu = listaUsuInters.cod_usu;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modPer == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = $"Proyecto - Permiso {tipoPerm.Descripcion}";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = proyecto.CodigoProyecto+"";
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "E";
                        notifProyecto.mensaje = $"Se eliminó Documento {dato.NombreDocumento} en el Permiso {tipoPerm.Descripcion} Expediente {existeProyecto.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                        notifProyecto.codigo = id;
                        notifProyecto.modulo = "P";
                        correosList.Add(correo);
                        notificacionList.Add(notifProyecto);
                    }
                }
                await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "E", $"Proyecto - Permiso {tipoPerm.Descripcion} Expediente {existeProyecto.Expediente} Documento {dato.NombreDocumento}", asunto);
                #endregion

                obj.Valid = true;
                obj.Message = Constantes.EliminacionSatisfactoria;
                return obj;
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
        }

        public async Task<PaginacionResponseDto<DocumentoPermisosResponseDTO>> Listar(ListDocumentosRequestDto listDocumentosRequestDto)
        {
            try
            {
                var dato = await _context.TipoPermisosProyecto.Where(x=>x.Descripcion.ToUpper().Equals(listDocumentosRequestDto.Modulo.ToUpper())).FirstOrDefaultAsync();
                if (dato!= null)
                {
                    var queryable = _context.DocumentosPermisos
                                .Where(x => listDocumentosRequestDto.Buscar != "" ? x.Expediente.Contains(listDocumentosRequestDto.Buscar) || x.NombreDocumento.Contains(listDocumentosRequestDto.Buscar) : true)
                                .Where(x=>x.TipoPermisosProyectoId == dato.Id)
                                .Where(x => x.PermisoId == listDocumentosRequestDto.PermisoId)
                                .Where(x=>x.Estado ==true)
                                .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoPermisosResponseDTO>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                else
                {
                    var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                    {
                        Cantidad = 0,
                        Model = null
                    };

                    return objeto;
                }
                
            }
            catch (Exception e)
            {

                var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                {
                    Cantidad = 0,
                    Model = null
                };
                return objeto;
            }
        }

        public async Task<ResponseEntidadDto<DocumentoPermisosResponseDTO>> Download(int id)
        {
            try
            {
                DocumentoPermisosResponseDTO obj = new DocumentoPermisosResponseDTO();
                var dato = await _context.DocumentosPermisos.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.ruta = dato.rutaFisica;
                obj.NombreDocumento = dato.NombreDocumento;
                obj.CodigoDocumento = dato.CodigoDocumento;

                return new ResponseEntidadDto<DocumentoPermisosResponseDTO>
                {
                    Model = obj,
                    Message = Constantes.RegistroExiste,
                    Valid = true
                };

            }
            catch (Exception)
            {

                return new ResponseEntidadDto<DocumentoPermisosResponseDTO>
                {
                    Model = null,
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false
                };
            }
           
        }

        public async Task<ResponseDTO> Update(int id, DatosUsuario usuario, PermisoUpdateDto dto)
        {
            
            var existePermiso = await _context.PermisosProyecto.Where(x => x.Id == id).FirstOrDefaultAsync();


            if (existePermiso == null)
            {
                return new ResponseDTO
                {
                    Message="No existe el permiso para editar",
                    Valid = false
                };
            }
            else
            {
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                var obtenerPermiso = await _context.TipoPermisosProyecto.Where(x => x.Id== existePermiso.TipoPermisosProyectoId).FirstOrDefaultAsync();
                var proyecto = await _context.Proyecto.Where(x=>x.Id == existePermiso.ProyectoId).FirstOrDefaultAsync();
                existePermiso.Longitud = dto.Longitud;
                existePermiso.Expediente = dto.Expediente;
                existePermiso.EstadoPermisosId = dto.EstadoPermisosId;
                existePermiso.UsuarioModifca = usuario.UsuaroId;
                existePermiso.FechaModificacion = DateTime.Now;
                List<CorreoTabla> camposModificados = CompararPropiedadesAsync(proyecto.CodigoProyecto,existePermiso, NomCompleto).GetAwaiter().GetResult();
                _context.Update(existePermiso);
                await _context.SaveChangesAsync();

                var documento = await _context.DocumentosPermisos.Where(x => x.PermisoId == existePermiso.Id).FirstOrDefaultAsync();

                if (documento != null)
                {
                    documento.Expediente = existePermiso.Expediente;
                    _context.Update(documento);
                    await _context.SaveChangesAsync();
                }

                Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Permisos";
                    trazabilidad.Evento = "Actualizar";
                    trazabilidad.DescripcionEvento = $"Se actualizó correctamente el permiso del proyecto {proyecto.CodigoProyecto} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                
                #region Envio de notificacion
                List<string> correosList1 = new List<string>();
                string asunto = $"Se actualizó el Permiso {obtenerPermiso.Descripcion} Expediente {existePermiso.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                foreach (var listaUsuInters in usuInt)
                {
                    int cod_usu = listaUsuInters.cod_usu;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modPer == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = $"Proyecto - Permiso {obtenerPermiso.Descripcion}";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = proyecto.CodigoProyecto;
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "M";
                        notifProyecto.mensaje = $"Se actualizó el Permiso {obtenerPermiso.Descripcion} Expediente {existePermiso.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                        notifProyecto.codigo = existePermiso.Id;
                        notifProyecto.modulo = "P";
                        correosList1.Add(correo);

                        var respuestNotif = await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                        dynamic objetoNotif = JsonConvert.DeserializeObject(respuestNotif.ToString());
                        int codigoNotifCreada = int.Parse(objetoNotif.codigoNot.ToString());
                        camposModificados.ForEach(item => {
                            item.idNotif = codigoNotifCreada;
                            item.id = null;
                            });
                        _context.CorreoTabla.AddRange(camposModificados);
                        _context.SaveChanges();
                    }
                }
                await _repositoryNotificaciones.EnvioCorreoNotifList(camposModificados, correosList1, "C", $"Proyecto - Permiso {obtenerPermiso.Descripcion}", asunto);
                #endregion


                var resultado = await _context.PermisosProyecto.Where(x => x.ProyectoId == existePermiso.ProyectoId && x.Estado == true).ToListAsync();


                decimal? resultadoLong = resultado.Sum(objeto => objeto.Longitud);
                var porcenaje = await _context.Constante.Where(x => x.Descripcion.Equals("% desviación")).FirstOrDefaultAsync();

                var LongAprobada = ((Convert.ToDecimal(porcenaje.valor) / 100) * proyecto?.LongAprobPa) + proyecto.LongAprobPa;

                var perfiles = await _context.Perfil.Where(x => x.nombre_perfil.Equals("Legal") || x.nombre_perfil.Equals("Administrador")).ToListAsync();
                List<string> correosList = new List<string>();
                List<Notificaciones> notificacionList = new List<Notificaciones>();
                List<Usuario> usuLegal = new List<Usuario>();
                if (perfiles != null)
                {
                    foreach (var item in perfiles)
                    {
                         var uzu = await _context.Usuario.Where(x => x.estado_user == "A" && x.Perfilcod_perfil == item.cod_perfil).ToListAsync();
                         usuLegal.AddRange(uzu);
                    }

                    if (resultadoLong > LongAprobada)
                    {
                        foreach (var item in usuLegal)
                        {
                            int cod_usu = item.cod_usu;
                            var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modPer == true).ToListAsync();
                            var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                            string correo = UsuarioInt[0].correo_usu.ToString();
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = $"Proyecto - Permiso";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = proyecto.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"La sumatoria de permisos ({String.Format("{0:#,##0.##}",Math.Round((decimal)resultadoLong, 2))} m) en el Proyecto {proyecto.CodigoProyecto} es mayor a la Longitud Aprobada del proyecto {String.Format("{0:#,##0.##}", Math.Round((decimal)proyecto.LongAprobPa, 2))} m";
                            notifProyecto.codigo = proyecto.Id;
                            notifProyecto.modulo = "P";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                        }
                    }
                }

                await _repositoryNotificaciones.CrearNotificacionList(notificacionList);

                return new ResponseDTO
                {
                    Message = Constantes.ActualizacionSatisfactoria,
                    Valid = true
                };
            }

            
        }

        public async Task<ResponseDTO> EliminarPermiso(int id, DatosUsuario usuario)
        {
            var resultado = await _context.PermisosProyecto.Where(x => x.Id == id).FirstOrDefaultAsync();
            var proyecto  = await _context.Proyecto.Where(x => x.Id == resultado.ProyectoId).FirstOrDefaultAsync();
            var permiso = await _context.TipoPermisosProyecto.Where(x => x.Id == resultado.TipoPermisosProyectoId).FirstOrDefaultAsync();
            if(resultado == null)
            {
                return new ResponseDTO
                {
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false
                };
            }
            else
            {
                resultado.Estado = false;
                resultado.UsuarioModifca = usuario.UsuaroId;
                resultado.FechaModificacion = DateTime.Now;
                _context.Update(resultado);
                await _context.SaveChangesAsync();

                var documentos = await _context.DocumentosPermisos.Where(x => x.PermisoId == id).ToListAsync();

                if (documentos.Count > 0)
                {
                    foreach (var item in documentos)
                    {
                        item.Estado = false;
                    }
                }
                _context.UpdateRange(documentos);
                await _context.SaveChangesAsync();
                Trazabilidad trazabilidad = new Trazabilidad();
                List<Trazabilidad> listaT = new List<Trazabilidad>();
                trazabilidad.Tabla = "Permisos";
                trazabilidad.Evento = "Eliminar";
                trazabilidad.DescripcionEvento = $"Se eliminó correctamente el permiso {permiso.Descripcion} del proyecto {proyecto.CodigoProyecto} ";
                trazabilidad.UsuarioId = usuario.UsuaroId;
                trazabilidad.DireccionIp = usuario.Ip;
                trazabilidad.FechaRegistro = DateTime.Now;

                listaT.Add(trazabilidad);
                await _trazabilidadRepository.Add(listaT);

                #region Envio de notificacion

                List<string> correosList = new List<string>();
                List<Notificaciones> notificacionList = new List<Notificaciones>();
                List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                CorreoTabla correoDatos = new CorreoTabla
                {
                    codigo = resultado.Id+""
                };
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string asunto = $"Se eliminó el Permiso {permiso.Descripcion} Expediente {resultado.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                composCorreo.Add(correoDatos);
                var usuInt = await _context.Usuario.Where(x => x.estado_user == "A").ToListAsync();
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                foreach (var listaUsuInters in usuInt)
                {
                    int cod_usu = listaUsuInters.cod_usu;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modPer == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = $"Proyecto - Permiso {permiso.Descripcion}";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = proyecto.CodigoProyecto+"";
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "E";
                        notifProyecto.mensaje = $"Se eliminó el Permiso {permiso.Descripcion} Expediente {resultado.Expediente} en Proyecto {proyecto.CodigoProyecto}";
                        notifProyecto.codigo = resultado.Id;
                        notifProyecto.modulo = "P";
                        correosList.Add(correo);
                        notificacionList.Add(notifProyecto);
                    }
                }
                await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "E", $"Proyecto - Permiso {permiso.Descripcion} Expediente {resultado.Expediente}", asunto);
                #endregion
                

                return new ResponseDTO
                {
                    Message = Constantes.EliminacionSatisfactoria,
                    Valid = true
                };
            }
        }
    }
}
