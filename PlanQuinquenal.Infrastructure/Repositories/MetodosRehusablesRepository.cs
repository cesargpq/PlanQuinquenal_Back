using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class MetodosRehusablesRepository:IRepositoryMetodosRehusables
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public MetodosRehusablesRepository(PlanQuinquenalContext context, IRepositoryNotificaciones repositoryNotificaciones)
        {
            _context = context;
            _repositoryNotificaciones = repositoryNotificaciones;
        }

        public async Task<object> CrearActa(ActaRequestDTO requestDoc, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            try {
                #region creacion del acta
                var entidadActas = new Actas
                {
                    fecha_reu = requestDoc.fecha_reu,
                    agenda = requestDoc.agenda,
                    acuerdos = requestDoc.acuerdos,
                    obj_reu = requestDoc.obj_reu,
                    compromisos = requestDoc.compromisos,
                    fecha_limComp = requestDoc.fecha_limComp,
                    cod_resp = requestDoc.cod_resp,
                    aprobacion = requestDoc.aprobacion,
                    cod_tipoSeg = requestDoc.cod_tipoSeg,
                    cod_seg = requestDoc.cod_seg,
                    fecha_emis = DateTime.Now
                };
                _context.Actas.Add(entidadActas);
                _context.SaveChanges();

                int idGeneradoActa = entidadActas.id;
                foreach (var listaUsuAsistActa in requestDoc.lstAsistActas)
                {
                    var entidad2 = new UsuAsisten_Acta
                    {
                        id_acta = idGeneradoActa,
                        cod_usu = listaUsuAsistActa.cod_usu
                    };
                    _context.UsuAsisten_Acta.Add(entidad2);
                }

                foreach (var listaUsuParticActa in requestDoc.lstPartActas)
                {
                    var entidad2 = new UsuParticip_Acta
                    {
                        id_acta = idGeneradoActa,
                        cod_usu = listaUsuParticActa.cod_usu
                    };
                    _context.UsuParticip_Acta.Add(entidad2);
                }

                foreach (var listaCamposDinam in requestDoc.lstCamposDinamic)
                {
                    var entidad2 = new CamposDinam_Acta
                    {
                        id_acta = idGeneradoActa,
                        nom_campo = listaCamposDinam.nom_campo,
                        valor_campo = listaCamposDinam.valor_campo
                    };
                    _context.CamposDinam_Acta.Add(entidad2);
                }
                _context.SaveChanges();
                #endregion

                #region Envio de notificacion
                var Proyecto = await _context.Proyectos.Where(x => x.id == requestDoc.cod_seg).ToListAsync();
                string cod_proy = Proyecto[0].cod_pry;

                Notificaciones notifInfoActas = new Notificaciones();
                notifInfoActas.cod_usu = idUser;
                notifInfoActas.seccion = "INFORMES Y ACTAS";
                notifInfoActas.nombreComp_usu = NomCompleto;
                notifInfoActas.cod_reg = requestDoc.cod_seg.ToString();
                notifInfoActas.area = nomPerfil;
                notifInfoActas.fechora_not = DateTime.Now;
                notifInfoActas.flag_visto = false;
                notifInfoActas.tipo_accion = "C";
                notifInfoActas.mensaje = $"Se creó el acta {idGeneradoActa} para el proyecto {cod_proy}" ;

                await _repositoryNotificaciones.CrearNotificacion(notifInfoActas);
                #endregion
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el acta correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0", 
                    mensaje = "Hubo un error al crear el acta"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            
        }

        public async Task<object> CrearComentario(Comentarios_proyec comentario, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            var entidad = new Comentarios_proyec
            {
                id_pry = comentario.id_pry,
                comentario = comentario.comentario,
                tipo_coment = comentario.tipo_coment,
                usuario = comentario.usuario,
                area = comentario.area,
                fecha_coment = DateTime.Now
            };
            _context.Comentarios_proyec.Add(entidad);
            _context.SaveChanges();

            #region Envio de notificacion
            var Proyecto = await _context.Proyectos.Where(x => x.id == comentario.id_pry).ToListAsync();
            string cod_proy = Proyecto[0].cod_pry;

            Notificaciones notifComentarios = new Notificaciones();
            notifComentarios.cod_usu = idUser;
            notifComentarios.seccion = "COMENTARIOS";
            notifComentarios.nombreComp_usu = NomCompleto; 
            notifComentarios.cod_reg = comentario.id_pry.ToString();
            notifComentarios.area = nomPerfil;
            notifComentarios.fechora_not = DateTime.Now;
            notifComentarios.flag_visto = false;
            notifComentarios.tipo_accion = "C";
            notifComentarios.mensaje = $"Se creó el comentario {entidad.id} para el proyecto {cod_proy}";

            await _repositoryNotificaciones.CrearNotificacion(notifComentarios);
            #endregion

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se creó el comentario correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> CrearDocumento(Docum_proyecto requestDoc, int idUser)
        {
            string fechaHoraActual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string NombreArch = requestDoc.nom_doc;
            string nombreFinal = NombreArch.Replace(".", $"_{fechaHoraActual}.");
            var entidad = new Docum_proyecto
            {
                id_pry = requestDoc.id_pry,
                nom_doc = nombreFinal,
                fecha_reg = DateTime.Now,
                mime_type = requestDoc.mime_type
            };
            _context.Docum_proyecto.Add(entidad);
            _context.SaveChanges();

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se creó el comentario correctamente",
                nombreDoc = nombreFinal
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> CrearInforme(InformeRequestDTO requestDoc, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            try {
                var entidad = new Informes
                {
                    fecha_inf = requestDoc.fecha_inf,
                    res_general = requestDoc.res_general,
                    prox_pasos = requestDoc.prox_pasos,
                    act_realiz = requestDoc.act_realiz,
                    cod_tipoSeg = requestDoc.cod_tipoSeg,
                    cod_seg = requestDoc.cod_seg,
                    fecha_emis = DateTime.Now
                };
                _context.Informes.Add(entidad);
                _context.SaveChanges();

                int idGenerado = entidad.id;
                foreach (var listaUsuInterInfo in requestDoc.lstUsuInterInformes)
                {
                    var entidad2 = new UsuariosIntersados_Inf
                    {
                        id_inf = idGenerado,
                        cod_usu = listaUsuInterInfo.cod_usu
                    };
                    _context.UsuariosIntersados_Inf.Add(entidad2);
                    _context.SaveChanges();
                }

                #region Envio de notificacion
                var Proyecto = await _context.Proyectos.Where(x => x.id == requestDoc.cod_seg).ToListAsync();
                string cod_proy = Proyecto[0].cod_pry;

                Notificaciones notifInfoActas = new Notificaciones();
                notifInfoActas.cod_usu = idUser;
                notifInfoActas.seccion = "INFORMES Y ACTAS";
                notifInfoActas.nombreComp_usu = NomCompleto;
                notifInfoActas.cod_reg = requestDoc.cod_seg.ToString();
                notifInfoActas.area = nomPerfil;
                notifInfoActas.fechora_not = DateTime.Now;
                notifInfoActas.flag_visto = false;
                notifInfoActas.tipo_accion = "C";
                notifInfoActas.mensaje = $"Se creó el informe {entidad.id} para el proyecto {cod_proy}";

                await _repositoryNotificaciones.CrearNotificacion(notifInfoActas);
                #endregion

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el informe correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el informe"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            
        }

        public async Task<object> CrearPermiso(Permisos_proyec requestDoc, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();

            string fechaHoraActual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string NombreArch = requestDoc.nom_doc;
            string nombreFinal = NombreArch.Replace(".", $"_{fechaHoraActual}.");
            var entidad = new Permisos_proyec
            {
                id_pry = requestDoc.id_pry,
                cod_tipoPerm = requestDoc.cod_tipoPerm,
                nom_doc = nombreFinal,
                num_exp = requestDoc.num_exp,
                fecha_reg = DateTime.Now,
                mime_type = requestDoc.mime_type
            };
            _context.Permisos_proyec.Add(entidad);
            _context.SaveChanges();

            #region Envio de notificacion
            var Proyecto = await _context.Proyectos.Where(x => x.id == requestDoc.id_pry).ToListAsync();
            string cod_proy = Proyecto[0].cod_pry;

            Notificaciones notifPermisos = new Notificaciones();
            notifPermisos.cod_usu = idUser;
            notifPermisos.seccion = "PERMISOS";
            notifPermisos.nombreComp_usu = NomCompleto;
            notifPermisos.cod_reg = requestDoc.id_pry.ToString();
            notifPermisos.area = nomPerfil;
            notifPermisos.fechora_not = DateTime.Now;
            notifPermisos.flag_visto = false;
            notifPermisos.tipo_accion = "C";
            notifPermisos.mensaje = $"Se creó el permiso {entidad.id} para el proyecto {cod_proy}";

            await _repositoryNotificaciones.CrearNotificacion(notifPermisos);
            #endregion

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se creó el permiso correctamente",
                nombreDoc = nombreFinal
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> EliminarActa(int codigo)
        {
            #region Eliminacion de Acta
            var regEliminar = _context.Actas.Find(codigo);
            _context.Actas.Remove(regEliminar);
            _context.SaveChanges();
            #endregion
            var registrosElimUsuAsit = _context.UsuAsisten_Acta.Where(registro => registro.id_acta == codigo);
            _context.UsuAsisten_Acta.RemoveRange(registrosElimUsuAsit);
            _context.SaveChanges();

            var registrosElimUsuPart = _context.UsuParticip_Acta.Where(registro => registro.id_acta == codigo);
            _context.UsuParticip_Acta.RemoveRange(registrosElimUsuPart);
            _context.SaveChanges();

            var registrosElimCampDin = _context.CamposDinam_Acta.Where(registro => registro.id_acta == codigo);
            _context.CamposDinam_Acta.RemoveRange(registrosElimCampDin);
            _context.SaveChanges();

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se eliminó el acta correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> EliminarComentario(int codigo)
        {
            var codComentEliminar = _context.Comentarios_proyec.Find(codigo);
            _context.Comentarios_proyec.Remove(codComentEliminar);
            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se eliminó el comentario correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> EliminarDocumento(int codigo)
        {
            var codDocElim = _context.Docum_proyecto.Find(codigo);
            _context.Docum_proyecto.Remove(codDocElim);
            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se eliminó el documento correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> EliminarInforme(int codigo)
        {
            #region Eliminacion de informe
            var regEliminar = _context.Informes.Find(codigo);
            _context.Informes.Remove(regEliminar);
            _context.SaveChanges();
            #endregion
            var registrosAEliminar = _context.UsuariosIntersados_Inf.Where(registro => registro.id_inf == codigo);
            _context.UsuariosIntersados_Inf.RemoveRange(registrosAEliminar);
            _context.SaveChanges();
            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se eliminó el informe correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> EliminarPermiso(int codigo)
        {
            var codPermEliminar = _context.Permisos_proyec.Find(codigo);
            _context.Permisos_proyec.Remove(codPermEliminar);
            _context.SaveChanges();
            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se eliminó el permiso correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> ModificarActa(ActaRequestDTO requestDoc, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            try
            { 
                var modActa = _context.Actas.FirstOrDefault(p => p.id == requestDoc.id);
                modActa.fecha_reu = requestDoc.fecha_reu;
                modActa.agenda = requestDoc.agenda;
                modActa.acuerdos = requestDoc.acuerdos;
                modActa.obj_reu = requestDoc.obj_reu;
                modActa.compromisos = requestDoc.compromisos;
                modActa.fecha_limComp = requestDoc.fecha_limComp;
                modActa.cod_resp = requestDoc.cod_resp;
                modActa.aprobacion = requestDoc.aprobacion;
                modActa.cod_tipoSeg = requestDoc.cod_tipoSeg;
                modActa.cod_seg = requestDoc.cod_seg;
                modActa.fecha_emis = requestDoc.fecha_emis;
                _context.SaveChanges();

                foreach (var listaUsuAsisActa in requestDoc.lstAsistActas)
                {
                    if (listaUsuAsisActa.tipoAccion == "C" && requestDoc.id == listaUsuAsisActa.id_acta)
                    {
                        var entidad2 = new UsuAsisten_Acta
                        {
                            id_acta = requestDoc.id,
                            cod_usu = listaUsuAsisActa.cod_usu
                        };
                        _context.UsuAsisten_Acta.Add(entidad2);
                        _context.SaveChanges();
                    }
                    else if (listaUsuAsisActa.tipoAccion == "E")
                    {
                        var regEliminar = _context.UsuAsisten_Acta.Find(listaUsuAsisActa.id);
                        _context.UsuAsisten_Acta.Remove(regEliminar);
                        _context.SaveChanges();
                    }

                }

                foreach (var listaUsuParticActa in requestDoc.lstPartActas)
                {
                    if (listaUsuParticActa.tipoAccion == "C" && requestDoc.id == listaUsuParticActa.id_acta)
                    {
                        var entidad2 = new UsuParticip_Acta
                        {
                            id_acta = requestDoc.id,
                            cod_usu = listaUsuParticActa.cod_usu
                        };
                        _context.UsuParticip_Acta.Add(entidad2);
                        _context.SaveChanges();
                    }
                    else if (listaUsuParticActa.tipoAccion == "E")
                    {
                        var regEliminar = _context.UsuParticip_Acta.Find(listaUsuParticActa.id);
                        _context.UsuParticip_Acta.Remove(regEliminar);
                        _context.SaveChanges();
                    }

                }

                foreach (var listaCamposDinam in requestDoc.lstCamposDinamic)
                {
                    if (listaCamposDinam.tipoAccion == "C" && requestDoc.id == listaCamposDinam.id_acta)
                    {
                        var entidad2 = new CamposDinam_Acta
                        {
                            id_acta = requestDoc.id,
                            nom_campo = listaCamposDinam.nom_campo,
                            valor_campo = listaCamposDinam.valor_campo
                        };
                        _context.CamposDinam_Acta.Add(entidad2);
                        _context.SaveChanges();
                    }
                    else if (listaCamposDinam.tipoAccion == "M")
                    {
                        var modCampoDin = _context.CamposDinam_Acta.FirstOrDefault(p => p.id == listaCamposDinam.id);
                        modCampoDin.nom_campo = listaCamposDinam.nom_campo;
                        modCampoDin.valor_campo = listaCamposDinam.valor_campo;
                        _context.SaveChanges();
                    }
                    else if (listaCamposDinam.tipoAccion == "E")
                    {
                        var regEliminar = _context.CamposDinam_Acta.Find(listaCamposDinam.id);
                        _context.CamposDinam_Acta.Remove(regEliminar);
                        _context.SaveChanges();
                    }

                }

                #region Envio de notificacion 
                var Proyecto = await _context.Proyectos.Where(x => x.id == requestDoc.cod_seg).ToListAsync();
                string cod_proy = Proyecto[0].cod_pry;

                Notificaciones notifInfoActas = new Notificaciones();
                notifInfoActas.cod_usu = idUser;
                notifInfoActas.seccion = "INFORMES Y ACTAS";
                notifInfoActas.nombreComp_usu = NomCompleto;
                notifInfoActas.cod_reg = requestDoc.cod_seg.ToString();
                notifInfoActas.area = nomPerfil;
                notifInfoActas.fechora_not = DateTime.Now;
                notifInfoActas.flag_visto = false;
                notifInfoActas.tipo_accion = "M";
                notifInfoActas.mensaje = $"Se modificó el acta {requestDoc.id} para el proyecto {cod_proy}";

                await _repositoryNotificaciones.CrearNotificacion(notifInfoActas);
                #endregion

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modificó el acta correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al modificar el acta"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            
        }

        public async Task<object> ModificarInforme(InformeRequestDTO requestDoc, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            try
            {
                var modInforme = _context.Informes.FirstOrDefault(p => p.id == requestDoc.id);
                modInforme.fecha_inf = requestDoc.fecha_inf;
                modInforme.res_general = requestDoc.res_general;
                modInforme.prox_pasos = requestDoc.prox_pasos;
                modInforme.act_realiz = requestDoc.act_realiz;
                modInforme.cod_tipoSeg = requestDoc.cod_tipoSeg;
                modInforme.cod_seg = requestDoc.cod_seg;
                modInforme.fecha_emis = requestDoc.fecha_emis;
                _context.SaveChanges();
                foreach (var listaUsuInterInfo in requestDoc.lstUsuInterInformes)
                {
                    if (listaUsuInterInfo.tipoAccion == "C")
                    {
                        if (requestDoc.id == listaUsuInterInfo.id_inf)
                        {
                            var entidad2 = new UsuariosIntersados_Inf
                            {
                                id_inf = requestDoc.id,
                                cod_usu = listaUsuInterInfo.cod_usu
                            };
                            _context.UsuariosIntersados_Inf.Add(entidad2);
                            _context.SaveChanges();
                        }
                    }
                    else if (listaUsuInterInfo.tipoAccion == "E")
                    {
                        var regEliminar = _context.UsuariosIntersados_Inf.Find(listaUsuInterInfo.id);
                        _context.UsuariosIntersados_Inf.Remove(regEliminar);
                        _context.SaveChanges();
                    }

                }

                #region Envio de notificacion
                var Proyecto = await _context.Proyectos.Where(x => x.id == requestDoc.cod_seg).ToListAsync();
                string cod_proy = Proyecto[0].cod_pry;

                Notificaciones notifInfoActas = new Notificaciones();
                notifInfoActas.cod_usu = idUser;
                notifInfoActas.seccion = "INFORMES Y ACTAS";
                notifInfoActas.nombreComp_usu = NomCompleto;
                notifInfoActas.cod_reg = requestDoc.cod_seg.ToString();
                notifInfoActas.area = nomPerfil;
                notifInfoActas.fechora_not = DateTime.Now;
                notifInfoActas.flag_visto = false;
                notifInfoActas.tipo_accion = "M";
                notifInfoActas.mensaje = $"Se modificó el informe {requestDoc.id} para el proyecto {cod_proy}"; 

                await _repositoryNotificaciones.CrearNotificacion(notifInfoActas);
                #endregion

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modificó el informe correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al modificar el informe"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            
        }
    }
}
