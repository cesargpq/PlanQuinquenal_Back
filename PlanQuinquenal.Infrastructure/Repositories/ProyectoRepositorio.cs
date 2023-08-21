using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanQuinquenal.Core.Utilities;
using ApiDavis.Core.Utilidades;
using System.Data.SqlTypes;
using PlanQuinquenal.Core.DTOs;
using OfficeOpenXml;
using System.Data;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ProyectoRepositorio : IProyectoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        
        private readonly IRepositoryMantenedores _repositoryMantenedores;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;
        public ProyectoRepositorio(PlanQuinquenalContext context, IMapper mapper,  IRepositoryMantenedores repositoryMantenedores , ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones )
        {
            this._context = context;
            this.mapper = mapper;
            this._repositoryNotificaciones = repositoryNotificaciones;
            this._repositoryMantenedores = repositoryMantenedores;
            this._trazabilidadRepository = trazabilidadRepository;
        }

        
       

        public async Task<ResponseDTO> Add(ProyectoRequestDto proyectoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                var existe = await _context.Proyecto.Where(x => x.CodigoProyecto == proyectoRequestDto.CodigoProyecto
                && x.TipoProy == (proyectoRequestDto.TipoProy == 1?true:false)
                && x.DistritoId == proyectoRequestDto.DistritoId
                && x.MaterialId == proyectoRequestDto.MaterialId
                && x.AñosPQ == proyectoRequestDto.AñosPQ
                ).AnyAsync();
                
                if (existe)
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.ExisteRegistro
                    };
                }
                else
                {
                    Proyecto proyecto = new Proyecto();
                    proyecto.CodigoProyecto = proyectoRequestDto.CodigoProyecto;
                    proyecto.PQuinquenalId = proyectoRequestDto.PQuinquenalId == 0 ? null : proyectoRequestDto.PQuinquenalId;
                    proyecto.AñosPQ = proyectoRequestDto.AñosPQ;
                    proyecto.PlanAnualId = proyectoRequestDto.PlanAnualId == 0 ? null : proyectoRequestDto.PlanAnualId;
                    proyecto.MaterialId = proyectoRequestDto.MaterialId == 0 ? null : proyectoRequestDto.MaterialId;
                    proyecto.ConstructorId = proyectoRequestDto.ConstructorId == 0 ? null : proyectoRequestDto.ConstructorId;
                    proyecto.TipoRegistroId = proyectoRequestDto.TipoRegistroId == 0 ? null : proyectoRequestDto.TipoRegistroId;
                    proyecto.LongAprobPa = proyectoRequestDto.LongAprobPa;
                    proyecto.InversionEjecutada = proyectoRequestDto.InversionEjecutada == null ? 0: proyectoRequestDto.InversionEjecutada;
                    proyecto.TipoProyectoId = proyectoRequestDto.TipoProyectoId == 0 ? null : proyectoRequestDto.TipoProyectoId;
                    proyecto.descripcion = "";
                    proyecto.CodigoMalla = proyectoRequestDto.CodigoMalla;
                    proyecto.IngenieroResponsableId = null;
                    proyecto.UsuarioRegisterId = usuario.UsuaroId;
                    proyecto.UsuarioModificaId = usuario.UsuaroId;
                    proyecto.FechaGasificacion = null;
                    proyecto.FechaRegistro = DateTime.Now;
                    proyecto.fechamodifica = DateTime.Now;
                    //proyecto.LongImpedimentos = 0;
                    proyecto.LongConstruida = 0;
                    proyecto.LongRealHab = 0;
                    proyecto.LongRealPend = 0;
                    proyecto.LongProyectos = 0;
                    proyecto.DistritoId = proyectoRequestDto.DistritoId;
                    proyecto.TipoProy = proyectoRequestDto.TipoProy==1?true:false;
                    _context.Add(proyecto);
                    await _context.SaveChangesAsync();

                    if (proyectoRequestDto.UsuariosInteresados.Count > 0)
                    {
                        List<UsuariosInteresadosPy> listPqUser = new List<UsuariosInteresadosPy>();
                        foreach (var item in proyectoRequestDto.UsuariosInteresados)
                        {
                            var existeUsu = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            if (existeUsu != null)
                            {
                                UsuariosInteresadosPy pqUser = new UsuariosInteresadosPy();
                                pqUser.CodigoProyecto = proyecto.CodigoProyecto;
                                pqUser.UsuarioId = item;
                                pqUser.Estado = true;
                                listPqUser.Add(pqUser);
                            }

                        }
                        _context.AddRange(listPqUser);
                        await _context.SaveChangesAsync();
                    }
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Proyectos , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad obj = new Trazabilidad();
                        List<Trazabilidad> lista = new List<Trazabilidad>();
                        obj.Tabla = "Proyecto";
                        obj.Evento = "Crear";
                        obj.DescripcionEvento = $"Creación de un nuevo proyecto {proyecto.CodigoProyecto}";
                        obj.UsuarioId = usuario.UsuaroId;
                        obj.DireccionIp = usuario.Ip;
                        obj.FechaRegistro = DateTime.Now;

                        lista.Add(obj);
                        await _trazabilidadRepository.Add(lista);
                    }




                    #region Comparacion de estructuras y agregacion de cambios

                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = proyecto.CodigoProyecto.ToString()
                    };

                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion

                    foreach (var listaUsuInters in proyectoRequestDto.UsuariosInteresados)
                    {
                        int cod_usu = listaUsuInters;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPry == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "PROYECTOS";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = proyectoRequestDto.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el proyecto {proyectoRequestDto.CodigoProyecto}";
                            notifProyecto.codigo = proyecto.Id;
                            notifProyecto.modulo = "P";

                            await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Proyectos");
                        }
                    }

                    #endregion
                    return new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.CreacionExistosa
                    };
                }
            }
            catch (Exception e )
            {

                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.ErrorSistema
                };
            }
        }

        public async Task<ResponseDTO> Update(ProyectoRequestUpdateDto p, int id, DatosUsuario usuario)
        {
            try
            {
                var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();

                var existe = await _context.Proyecto.Where(x => x.Id == id).FirstOrDefaultAsync();
                    List<CorreoTabla> camposModificados;
                    if (existe != null)
                    {
                        existe.descripcion = p.Descripcion;
                        existe.PQuinquenalId = p.PQuinquenalId;
                        existe.AñosPQ = p.AñosPQ;
                        existe.PlanAnualId = p.PlanAnualId == null || p.PlanAnualId == 0 ? null : p.PlanAnualId;
                        existe.MaterialId = p.MaterialId == null || p.MaterialId == 0 ? null : p.MaterialId;
                        existe.DistritoId = p.DistritoId == null || p.DistritoId == 0 ? null : p.DistritoId;
                        existe.TipoProyectoId = p.TipoProyectoId == null || p.TipoProyectoId == 0 ? null : p.TipoProyectoId;
                        existe.CodigoMalla = p.CodigoMalla;
                        existe.TipoRegistroId = p.TipoRegistroId == null || p.TipoRegistroId == 0 ? null : p.TipoRegistroId;
                        existe.IngenieroResponsableId = p.IngenieroResponsableId == null || p.IngenieroResponsableId == 0 ? null : p.IngenieroResponsableId;
                        existe.ConstructorId = p.ConstructorId == null || p.ConstructorId == 0 ? null : p.ConstructorId;
                        existe.InversionEjecutada = p.InversionEjecutada;
                        existe.FechaGasificacion = p.FechaGacificacion != "" ? DateTime.Parse(p.FechaGacificacion) : null;
                        existe.LongAprobPa = p.LongAprobPa;
                        existe.LongRealPend = p.LongRealPend;
                        existe.LongRealHab = p.LongRealHab;
                        existe.LongProyectos = p.LongProyectos;
                        existe.UsuarioModificaId = usuario.UsuaroId;
                        existe.fechamodifica = DateTime.Now;

                        camposModificados = CompararPropiedadesAsync(existe, NomCompleto).GetAwaiter().GetResult();
                       
                        _context.Update(existe);
                        await _context.SaveChangesAsync();


                    if (p.UsuariosInteresados.Count > 0)
                        {
                            var userInt = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto == existe.CodigoProyecto).ToListAsync();

                            bool todosExisten = p.UsuariosInteresados.All(valor => userInt.Any(objeto => objeto.UsuarioId  == valor));
                            bool hayValoresExtras = userInt.Any(objeto => !p.UsuariosInteresados.Contains(objeto.UsuarioId ));
                            if (!todosExisten || hayValoresExtras)
                            {
                                var listaUsuarios = await _repositoryMantenedores.GetAllByAttribute("Usuario");
                                var original = listaUsuarios.Where(objeto => userInt.Find(usu=>usu.UsuarioId  == objeto.Id) != null)
                                                                                .Select(objeto => objeto.Descripcion) 
                                                                                .ToList();
                                var modificado = listaUsuarios.Where(objeto => p.UsuariosInteresados.Contains(objeto.Id))
                                                                                .Select(objeto => objeto.Descripcion) 
                                                                                .ToList();
                                camposModificados.Add(CrearCorreoTablaManual(existe.CodigoProyecto,string.Join(", ", original),string.Join(", ", modificado),NomCompleto,DateTime.Today.ToString("dd/MM/yyyy"),"Usuarios Interesados"));
                            }

                            foreach (var item in userInt)
                            {
                                _context.Remove(item);
                                await _context.SaveChangesAsync();
                            }
                            List<UsuariosInteresadosPy> listPqUser = new List<UsuariosInteresadosPy>();
                            foreach (var item in p.UsuariosInteresados)
                            {
                                var existeUsu = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                                if (existeUsu != null)
                                {
                                    UsuariosInteresadosPy pqUser = new UsuariosInteresadosPy();
                                    pqUser.CodigoProyecto = existe.CodigoProyecto;
                                    pqUser.UsuarioId = item;
                                    pqUser.Estado = true;
                                    listPqUser.Add(pqUser);
                                }
                            }
                            foreach (var item in listPqUser)
                            {
                                _context.Add(item);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Proyectos , Crear").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Proyecto";
                            trazabilidad.Evento = "Editar";
                            trazabilidad.DescripcionEvento = $"Se actualizó correctamente el proyecto {existe.CodigoProyecto} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }


                    #region notificacion

                    


                    foreach (var listaUsuInters in p.UsuariosInteresados)
                    {
                        int cod_usu = listaUsuInters;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modPry == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "PROYECTOS";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = p.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "M";
                            notifProyecto.mensaje = $"Se modificó el proyecto {p.CodigoProyecto}";
                            notifProyecto.codigo = existe.Id;
                            notifProyecto.modulo = "P";

                            var respuestNotif = await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            dynamic objetoNotif = JsonConvert.DeserializeObject(respuestNotif.ToString());
                            int codigoNotifCreada = int.Parse(objetoNotif.codigoNot.ToString());
                            await _repositoryNotificaciones.EnvioCorreoNotif(camposModificados, correo, "M", "Proyectos");
                            camposModificados.ForEach(item => {
                                item.idNotif = codigoNotifCreada;
                                item.id = null;
                                });
                            _context.CorreoTabla.AddRange(camposModificados);
                            _context.SaveChanges();
                        }
                    }

                    #endregion



                    var objeto = new ResponseDTO
                        {
                            Message = Constantes.ActualizacionSatisfactoria,
                            Valid = true
                        };
                        return objeto;
                    }
                    else
                    {
                        var objeto = new ResponseDTO
                        {
                            Message = Constantes.BusquedaNoExitosa,
                            Valid = true
                        };
                        return objeto;
                    }

                

            }
            catch (Exception e)
            {

                var objeto = new ResponseDTO
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false
                };
                return objeto;
            }
        }
        public async Task<List<CorreoTabla>> CompararPropiedadesAsync(object valOriginal, string nomCompleto)
        {
            List<CorreoTabla> camposModificados = new List<CorreoTabla>();
            if (valOriginal!= null)
            {
                    
                
                DateTime fechaActual = DateTime.Today;
                string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
                var compara = _context.Entry(valOriginal).Properties.Where(p=>p.IsModified).ToList();

                foreach (var item in compara)
                {
                    if (item.Metadata.Name.Equals("fechamodifica") || item.Metadata.Name.Equals("UsuarioModificaId"))
                    {
                        continue;
                    }
                    else if (item.Metadata.Name.Equals("PQuinquenalId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Plan Quinquenal");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("PlanAnualId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("PlanAnual");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Plan Anual");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("MaterialId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Material");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Material");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("DistritoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Distrito");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Distrito");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("ConstructorId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Constructor");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Constructor");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("TipoProyectoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("TipoProyecto");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Tipo Proyecto");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("TipoRegistroId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("TipoRegistro");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Tipo Registro");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("IngenieroResponsableId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Usuario");
                        CorreoTabla fila = CrearCorreoTabla(valOriginal, nomCompleto, fechaFormateada, item, enumerable, "Ingeniero Responsable");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("FechaGasificacion"))
                    {
                        CorreoTabla fila = CrearCorreoTablaManualFecha(valOriginal,  item, nomCompleto, fechaFormateada, "Fecha Gasificación");
                        camposModificados.Add(fila);
                    }
                    else
                    {
                        CorreoTabla fila = CrearCorreoTablaDirecto(valOriginal, nomCompleto, fechaFormateada, item, SepararPalabrasConMayusculas(item.Metadata.Name));
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

        private static CorreoTabla CrearCorreoTablaManualFecha(object codigo,PropertyEntry? item, string nomCompleto, string fechaFormateada,string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = (codigo as Proyecto).CodigoProyecto,
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

        private static CorreoTabla CrearCorreoTabla(object valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, IEnumerable<MaestroResponseDto> enumerable, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = (valOriginal as Proyecto).CodigoProyecto,
                valorActual = item.OriginalValue!= null ? enumerable.Where(x => x.Id == (int)item.OriginalValue).FirstOrDefault().Descripcion: "",
                campoModificado = nombreColumna,
                valorModificado =item.CurrentValue!=null? enumerable.Where(x => x.Id == (int)item.CurrentValue).FirstOrDefault().Descripcion: "",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTablaDirecto(object valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = (valOriginal as Proyecto).CodigoProyecto,
                valorActual =item.OriginalValue!= null ? item.OriginalValue +"" :"",
                campoModificado = nombreColumna,
                valorModificado = item.CurrentValue!= null ? item.CurrentValue +"" :"",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        public async Task<PaginacionResponseDtoException<ProyectoDetalle>> GetAll2(FiltersProyectos f)
        {
            if (f.CodigoProyecto.Equals("")) f.CodigoProyecto = null;
            if (f.NroExpediente.Equals("")) f.NroExpediente = null;
            if (f.AñoPq.Equals("")) f.AñoPq = null;
            if (f.CodigoMalla.Equals("")) f.CodigoMalla = null;
            if (f.NombreProyecto.Equals("")) f.NombreProyecto = null;
            if (f.ProblematicaReal.Equals("")) f.ProblematicaReal = null;
            if (f.FechaGasificacion.Equals("")) f.FechaGasificacion = null;
            //else
            //{
            //    string[] cadena = f.FechaGasificacion.Split("-");

            //    f.FechaGasificacion = cadena[2] + "-" + cadena[1] +"-" + cadena[0];
            //}
            if (f.EstadoGeneral == 0) f.EstadoGeneral = null;
            
            if (f.MaterialId == 0) f.MaterialId = null;
            if (f.DistritoId == 0) f.DistritoId = null;
            if (f.TipoProyectoId == 0) f.TipoProyectoId = null;
            if (f.PQuinquenalId == 0) f.PQuinquenalId = null;
            if (f.PAnualId == 0) f.PAnualId = null;
            if (f.ConstructorId == 0) f.ConstructorId = null;
            if (f.IngenieroId == 0) f.IngenieroId = null;
            if (f.UsuarioRegisterId == 0) f.UsuarioRegisterId = null;



            var resultad = await _context.ProyectoDetalle.FromSqlInterpolated($"EXEC listar {f.CodigoProyecto} , {f.NroExpediente} , {f.AñoPq} , {f.CodigoMalla} , {f.NombreProyecto} , {f.ProblematicaReal} , {f.FechaGasificacion} , {f.EstadoGeneral} , {f.MaterialId} , {f.DistritoId} , {f.TipoProyectoId} , {f.PQuinquenalId} , {f.PAnualId} , {f.ConstructorId} , {f.IngenieroId} , {f.UsuarioRegisterId} , {f.TipoProy}, {f.Pagina} , {f.RecordsPorPagina}").ToListAsync();


            var dato = new PaginacionResponseDtoException<ProyectoDetalle>
            {
                Cantidad = resultad.Count() == 0 ? 0 : resultad.ElementAt(0).Total,
                Model = resultad
            };
            return dato;
        }

        public async Task<ImportResponseDto<Proyecto>> ProyectoImport(RequestMasivo data, DatosUsuario usuario)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();

            ImportResponseDto<Proyecto> dto = new ImportResponseDto<Proyecto>();
            var Material = await _repositoryMantenedores.GetAllByAttribute(Constantes.Material);
            var Constructor = await _repositoryMantenedores.GetAllByAttribute(Constantes.Constructor);
            var TipoProyecto = await _repositoryMantenedores.GetAllByAttribute(Constantes.TipoProyecto);
            var Distrito = await _repositoryMantenedores.GetAllByAttribute(Constantes.Distrito);
            var TipoRegistroPY = await _repositoryMantenedores.GetAllByAttribute(Constantes.TipoRegistroPY);
            var PlanQuin = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");
            var PlanAnu = await _repositoryMantenedores.GetAllByAttribute("PlanAnual");
            var proyectosMasivos = await _context.ProyectoMasivoDetalle.FromSqlInterpolated($"EXEC listaMasiva").ToListAsync();

            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            List<Proyecto> lista = new List<Proyecto>();
            List<Proyecto> listaError = new List<Proyecto>();
            List<Proyecto> listaRepetidos = new List<Proyecto>();
            List<Proyecto> listaInsert = new List<Proyecto>();
            List<Proyecto> listaRepetidosInsert = new List<Proyecto>();

            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Proyecto"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var codPry = worksheet.Cells[row, 1].Value?.ToString();
                            var codPQ = worksheet.Cells[row, 2].Value?.ToString();
                            var anioPQ = worksheet.Cells[row, 3].Value?.ToString();
                            var anioPA = worksheet.Cells[row, 4].Value?.ToString();
                            var material = worksheet.Cells[row, 5].Value?.ToString();
                            var constructor = worksheet.Cells[row, 6].Value?.ToString();
                            var tipoRegistro = worksheet.Cells[row, 7].Value?.ToString();
                            var distrito = worksheet.Cells[row, 8].Value?.ToString();
                            var tipoProyecto = worksheet.Cells[row, 9].Value?.ToString();
                            var longAproPa = worksheet.Cells[row, 10].Value?.ToString();
                            var longRealHab = worksheet.Cells[row, 11].Value?.ToString();
                            var longRealPend = worksheet.Cells[row, 12].Value?.ToString();
                            var codMalla = worksheet.Cells[row, 13].Value?.ToString();
                            var InversionEjecutada = worksheet.Cells[row, 14].Value?.ToString();

                            var dPQ = PlanQuin.Where(x => x.Descripcion.Contains(codPQ)).FirstOrDefault();
                            var PlanAnualId = PlanAnu.Where(x => x.Descripcion.Contains(anioPA)).FirstOrDefault();
                            var dMaterial = Material.Where(x => x.Descripcion.Contains(material)).FirstOrDefault();
                            var dConstructor = Constructor.Where(x => x.Descripcion.Contains(constructor)).FirstOrDefault();
                            var dTipoProyecto = TipoProyecto.Where(x => x.Descripcion.Contains(tipoProyecto)).FirstOrDefault();
                            var dDistrito = Distrito.Where(x => x.Descripcion.Contains(distrito)).FirstOrDefault();
                            var dTipoRegistroPY = TipoRegistroPY.Where(x => x.Descripcion.Contains(tipoRegistro)).FirstOrDefault();

                         
                            


                            if (dPQ == null  || dMaterial == null || dConstructor == null || dTipoProyecto == null || dDistrito == null)
                            {
                                var entidadError = new Proyecto
                                {
                                    CodigoProyecto = codPry,

                                };
                                listaError.Add(entidadError);

                            }
                            else
                            {
                                try
                                {
                                    var entidad = new Proyecto
                                    {
                                        CodigoProyecto = codPry,
                                        PQuinquenalId = dPQ.Id,
                                        AñosPQ = anioPQ,
                                        PlanAnualId = PlanAnualId.Id,
                                        MaterialId = dMaterial.Id,
                                        ConstructorId = dConstructor.Id,
                                        TipoRegistroId = dTipoRegistroPY.Id,
                                        DistritoId = dDistrito.Id,
                                        TipoProyectoId = dTipoProyecto.Id,
                                        LongAprobPa = longAproPa == null ? 0 : Decimal.Parse(longAproPa),
                                        LongRealHab = longRealHab == null ? 0 : Decimal.Parse(longRealHab),
                                        LongRealPend = longRealPend == null ? 0 : Decimal.Parse(longRealPend),
                                        LongProyectos = 0,
                                        LongConstruida=0,
                                        //LongImpedimentos=0,
                                        //LongReemplazada=0,
                                        //LongImpedimentos = Decimal.Parse(longImpedimentos),
                                        //LongReemplazada = Decimal.Parse(longReemplazada),
                                        CodigoMalla = codMalla,
                                        InversionEjecutada = InversionEjecutada==null?0:Decimal.Parse(InversionEjecutada),
                                        FechaRegistro = DateTime.Now,
                                        fechamodifica = DateTime.Now,
                                        UsuarioRegisterId = usuario.UsuaroId,
                                        UsuarioModificaId = usuario.UsuaroId

                                    };
                                    lista.Add(entidad);
                                }
                                catch (Exception e)
                                {
                                    var entidadError = new Proyecto
                                    {
                                        CodigoProyecto = codPry,

                                    };
                                    listaError.Add(entidadError);

                                }

                            }


                        }
                        foreach (var item in lista)
                        {
                            try
                            {
                                var existes = proyectosMasivos.Where(x => x.CodigoProyecto.Equals(item.CodigoProyecto) 
                                && x.TipoProy == (item.TipoProy ==true ?1:0)
                                && x.MaterialId == item.MaterialId
                                && x.DistritoId == item.DistritoId
                                && x.AñosPQ == item.AñosPQ
                                ).FirstOrDefault();

                                if (existes != null)
                                {
                                    var repetidos = new Proyecto
                                    {
                                        CodigoProyecto = existes.CodigoProyecto,
                                        Id = existes.Id
                                    };
                                    listaRepetidos.Add(repetidos);
                                    Proyecto existe = new Proyecto();
                                    existe.Id = existes.Id;
                                    existe.CodigoProyecto = existes.CodigoProyecto;
                                    existe.descripcion = existes.descripcion;
                                    existe.PQuinquenalId = item.PQuinquenalId;
                                    existe.AñosPQ = item.AñosPQ;
                                    existe.PlanAnualId = item.PlanAnualId;
                                    existe.MaterialId = item.MaterialId;
                                    existe.DistritoId = item.DistritoId;
                                    existe.TipoProyectoId = item.TipoProyectoId;
                                    existe.CodigoMalla = item.CodigoMalla;
                                    existe.TipoRegistroId = item.TipoRegistroId;
                                    existe.IngenieroResponsableId = existes.IngenieroResponsableId;
                                    existe.ConstructorId = item.ConstructorId;
                                    existe.InversionEjecutada = item.InversionEjecutada;
                                    existe.UsuarioRegisterId = existes.UsuarioRegisterId;
                                    existe.UsuarioModificaId = existes.UsuarioModificaId;
                                    existe.FechaRegistro = existes.FechaRegistro;
                                    existe.fechamodifica = DateTime.Now;
                                    existe.FechaGasificacion = existes.FechaGasificacion;
                                    existe.LongAprobPa = item.LongAprobPa;
                                    existe.LongRealHab = item.LongRealHab;
                                    existe.LongRealPend = item.LongRealPend;
                                    existe.LongConstruida = existe.LongConstruida;
                                    //existe.LongImpedimentos = existes.LongImpedimentos;
                                    //existe.LongReemplazada = existes.LongReemplazada;
                                    existe.LongProyectos = existes.LongProyectos;
                                    existe.UsuarioModificaId = usuario.UsuaroId;
                                    listaRepetidosInsert.Add(existe);

                                }
                                else
                                {
                                    item.TipoProy = data.TipoProy == 1 ? true : false;
                                    listaInsert.Add(item);
                                }
                            }
                            catch (Exception e)
                            {

                                var entidadError = new Proyecto
                                {
                                    CodigoProyecto = item.CodigoProyecto
                                };
                                listaError.Add(entidadError);
                            }


                        }
                        if (listaInsert.Count > 0)
                        {
                            await _context.BulkInsertAsync(listaInsert);
                            await _context.SaveChangesAsync();
                        }
                        if (listaRepetidosInsert.Count > 0)
                        {
                            _context.BulkUpdate(listaRepetidosInsert);

                        }


                    }
                }

                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Proyectos , Crear").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Proyecto";
                    trazabilidad.Evento = "Crear Masivo";
                    trazabilidad.DescripcionEvento = $"Se insertó masivamente {listaInsert.Count()} proyectos, se actualizó correctamente {listaRepetidos.Count()} proyectos , se insertó con error {listaError.Count()} proyectos";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }



                #region Comparacion de estructuras y agregacion de cambios

                
                List< Notificaciones >  notificacioneslist= new List<Notificaciones>();
                List<Notificaciones> notificacioneslistrepetido = new List<Notificaciones>();
                foreach (var item in listaInsert)
                {
                   
                    Notificaciones notifProyecto = new Notificaciones();
                    notifProyecto.cod_usu = usuario.UsuaroId;
                    notifProyecto.seccion = "PROYECTOS";
                    notifProyecto.nombreComp_usu = NomCompleto;
                    notifProyecto.cod_reg = item.CodigoProyecto;
                    notifProyecto.area = nomPerfil;
                    notifProyecto.fechora_not = DateTime.Now;
                    notifProyecto.flag_visto = false;
                    notifProyecto.tipo_accion = "C";
                    notifProyecto.mensaje = $"Se creó el proyecto {item.CodigoProyecto}";
                    notifProyecto.codigo = item.Id;
                    notifProyecto.modulo = "P";
                    notificacioneslist.Add(notifProyecto);
                }
                if(notificacioneslist.Count > 0)
                {
                    await _context.BulkInsertAsync(notificacioneslist);
                    await _context.SaveChangesAsync();
                }
                foreach (var item in listaRepetidos)
                {
                    Notificaciones notifProyecto = new Notificaciones();
                    notifProyecto.cod_usu = usuario.UsuaroId;
                    notifProyecto.seccion = "PROYECTOS";
                    notifProyecto.nombreComp_usu = NomCompleto;
                    notifProyecto.cod_reg = item.CodigoProyecto;
                    notifProyecto.area = nomPerfil;
                    notifProyecto.fechora_not = DateTime.Now;
                    notifProyecto.flag_visto = false;
                    notifProyecto.tipo_accion = "M";
                    notifProyecto.mensaje = $"Se editó el proyecto {item.CodigoProyecto}";
                    notifProyecto.codigo = item.Id;
                    notifProyecto.modulo = "P";
                    notificacioneslistrepetido.Add(notifProyecto);
                }
                if (notificacioneslistrepetido.Count > 0)
                {
                    await _context.BulkInsertAsync(notificacioneslistrepetido);
                    await _context.SaveChangesAsync();
                }

                #endregion

                #region Envio de notificacion

                #endregion


                dto.listaError = listaError;
                dto.listaRepetidos = listaRepetidos;
                dto.listaInsert = listaInsert;
                dto.Satisfactorios = listaInsert.Count();
                dto.Error = listaError.Count();
                dto.Actualizados = listaRepetidos.Count();
                dto.Valid = true;
                dto.Message = Constantes.SatisfactorioImport;
            }
            catch (Exception e)
            {
                dto.Satisfactorios = 0;
                dto.Error = 0;
                dto.Actualizados = 0;
                dto.Valid = false;
                dto.Message = Constantes.ErrorImport;

                return dto;
            }

            return dto;

        }

        public static DataTable ToDataTable(IEnumerable<Proyecto> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Proyecto));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        public async Task<ResponseEntidadDto<ProyectoResponseIdDTO>> GetById(int Id)
        {
            var resultad = await _context.ProyectoResponseIdDTO.FromSqlRaw($"EXEC listarpybyid  {Id}").ToListAsync();
            var usuariosInteresados = await _context.UsuariosInteresadosPy
                                            .Include(x=>x.Usuario)    
                                            .Where(x => x.CodigoProyecto.Equals(resultad.ElementAt(0).CodigoProyecto)).ToListAsync();

            List<UsuarioResponseDto> obj = new List<UsuarioResponseDto>();

            foreach (var item in usuariosInteresados)
            {
                UsuarioResponseDto o = new UsuarioResponseDto();
                o.Id = item.UsuarioId;
                o.Nombre = item.Usuario.nombre_usu + " " + item.Usuario.apellido_usu;
                obj.Add(o);
            }
            if (resultad.Count >0)
            {
                resultad.ElementAt(0).UsuariosInteresados = obj;
                var result = new ResponseEntidadDto<ProyectoResponseIdDTO>
                {
                    Message = Constantes.BusquedaExitosa,
                    Valid = true,
                    Model = resultad[0]
                };
                return result;
            }
            else
            {
                var result = new ResponseEntidadDto<ProyectoResponseIdDTO>
                {
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false,
                    Model = null
                };
                return result;
            }
        }

        public async Task<PaginacionResponseDtoException<ProyectoDetalle>> GetSeleccionados(PlanQuinquenalSelectedId p)
        {
            List<int> intList = new List<int> { 1, 2, 3, 4, 5 };

            var dataTable = new DataTable();
            dataTable.TableName = "dbo.IntArray";
            dataTable.Columns.Add("Id", typeof(int));
            foreach (var item in p.Value)
            {
                dataTable.Rows.Add(item);
            }

            SqlParameter parameter = new SqlParameter("IntArray", SqlDbType.Structured);
            parameter.TypeName = dataTable.TableName;
            parameter.Value = dataTable;


            var resultad = await _context.ProyectoDetalle.FromSqlInterpolated($"EXEC ListarPYIndividual {parameter}").ToListAsync();

            var result = new PaginacionResponseDtoException<ProyectoDetalle>
            {
                Cantidad = resultad.Count,
                Model = resultad
            };
            return result;
        }

        public async Task<PaginacionResponseDtoException<ProyectoEtapaResponseDto>> ListarEtapas(EtapasListDto filterProyectos)
        {
            var resultad = await _context.ProyectoEtapaResponseDto.FromSqlInterpolated($"EXEC ListarEtapas {filterProyectos.ProyectoId}").ToListAsync();
            if (resultad.Count > 0)
            {
                var result = new PaginacionResponseDtoException<ProyectoEtapaResponseDto>
                {
                    Cantidad = resultad.Count,
                    Model = resultad
                };
                return result;
            }
            else
            {
                var result = new PaginacionResponseDtoException<ProyectoEtapaResponseDto>
                {
                    Cantidad = 0,
                    Model = null
                };
                return result;
            }
            

        }
    }
}
