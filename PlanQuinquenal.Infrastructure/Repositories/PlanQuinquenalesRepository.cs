using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PlanQuinquenalesRepository : IPlanQuinquenalesRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;
        private readonly IRepositoryMetodosRehusables _repositoryMetodosRehusables;
        private readonly IMapper mapper;
        public PlanQuinquenalesRepository(PlanQuinquenalContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this._context = context;
        }

        public async Task<bool> CreatePQ(PQuinquenalReqDTO pQuinquenalReqDTO, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();

            var pq = mapper.Map<PlanQuinquenal.Core.Entities.PlanQuinquenal>(pQuinquenalReqDTO);
            _context.Add(pq);
            await _context.SaveChangesAsync();

            var pqFirst = await _context.PlanQuinquenal.Where(x => x.Pq == pQuinquenalReqDTO.Pq).FirstOrDefaultAsync();
            
            List<PQUsuariosInteresados> listPqUser = new List<PQUsuariosInteresados>();
            foreach (var item in pQuinquenalReqDTO.IdUsuario)
            {
                PQUsuariosInteresados pqUser = new PQUsuariosInteresados();
                pqUser.PlanQuinquenalId = pqFirst.Id;
                pqUser.UsuarioId = item;
                pqUser.Estado = true;
                listPqUser.Add(pqUser);
            }
            //listPqUser.ForEach(n => _context.Add(n));
            _context.AddRange(listPqUser);
            await _context.SaveChangesAsync();

            #region Comparacion de estructuras y agregacion de cambios

            List<CorreoTabla> composCorreo = new List<CorreoTabla>();
            CorreoTabla correoDatos = new CorreoTabla
            {
                codigo = pqFirst.Id.ToString()
            };

            composCorreo.Add(correoDatos);
            #endregion

            #region Envio de notificacion

            foreach (var item in pQuinquenalReqDTO.IdUsuario)
            {
                int cod_usuReg = item;
                var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usuReg).Where(x => x.regPQ == true).ToListAsync();
                var UsuarioInt = await _context.Usuario.Include(y => y.Perfil).Where(y => y.cod_usu == cod_usuReg).ToListAsync();
                string correo = UsuarioInt[0].correo_usu.ToString();
                if (lstpermisos.Count() == 1)
                {
                    Notificaciones notifPQ = new Notificaciones();
                    notifPQ.cod_usu = cod_usuReg;
                    notifPQ.seccion = "PLANQUINQUENAL";
                    notifPQ.nombreComp_usu = NomCompleto;
                    notifPQ.cod_reg = pqFirst.Id.ToString();
                    notifPQ.area = nomPerfil;
                    notifPQ.fechora_not = DateTime.Now;
                    notifPQ.flag_visto = false;
                    notifPQ.tipo_accion = "C";
                    notifPQ.mensaje = $"Se creó el plan quinquenal {pqFirst.Id}";
                    notifPQ.codigo = pqFirst.Id;
                    notifPQ.modulo = "PQ";

                    await _repositoryNotificaciones.CrearNotificacion(notifPQ);
                    await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Plan Quinquenal");
                }
            }

            #endregion

            return true;
        }

        public async Task<Object> ActualizarPQ(PQuinquenalReqDTO planquinquenalReq, int idUser)
        {
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            string nomPerfil = Usuario[0].Perfil.nombre_perfil;
            string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
            string fechaHoraActual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var PQOriginal = await _context.PlanQuinquenal.Where(x => x.Id == planquinquenalReq.Id).ToListAsync();
            try
            {
                var modPQ = _context.PlanQuinquenal.FirstOrDefault(p => p.Id == planquinquenalReq.Id);
                modPQ.Pq = planquinquenalReq.Pq;
                modPQ.EstadoId = planquinquenalReq.EstadoId;
                modPQ.Aprobaciones = planquinquenalReq.Aprobaciones;
                modPQ.Descripcion = planquinquenalReq.Descripcion;

                #region MODULO DE USUARIOS INTERESADOS

                foreach (var listaUsuInter in planquinquenalReq.lstUsuaInter)
                {
                    if (listaUsuInter.TipoAccion == "C")
                    {
                        #region Creacion de usuario interesado
                        var entidad = new PQUsuariosInteresados
                        {
                            PlanQuinquenalId = planquinquenalReq.Id,
                            UsuarioId = listaUsuInter.UsuarioId
                        };
                        _context.PQUsuariosInteresados.Add(entidad);
                        #endregion
                    }
                    else if (listaUsuInter.TipoAccion == "E")
                    {
                        #region Eliminacion de usuario interesado
                        var codUsuInterEliminar = _context.PQUsuariosInteresados.Find(listaUsuInter.UsuarioId);
                        _context.PQUsuariosInteresados.Remove(codUsuInterEliminar);
                        #endregion
                    }
                }
                #endregion

                _context.SaveChanges();

                #region Comparacion de estructuras y agregacion de cambios

                PlanQuinquenal.Core.Entities.PlanQuinquenal proyectoModificado = new PlanQuinquenal.Core.Entities.PlanQuinquenal
                {
                    Id = planquinquenalReq.Id,
                    Pq = planquinquenalReq.Pq,
                    EstadoId = planquinquenalReq.EstadoId,
                    Aprobaciones = planquinquenalReq.Aprobaciones,
                    Descripcion = planquinquenalReq.Descripcion
            };

                List<CorreoTabla> camposModificados = CompararPropiedades(PQOriginal[0], proyectoModificado, planquinquenalReq.Id.ToString(), NomCompleto);
                #endregion

                #region Envio de notificacion

                foreach (var listaUsuInters in planquinquenalReq.lstUsuaInter) 
                {
                    int cod_usuReg = listaUsuInters.UsuarioId;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usuReg).Where(x => x.modPQ == true).ToListAsync();
                    var UsuarioInt = await _context.Usuario.Include(y => y.Perfil).Where(y => y.cod_usu == cod_usuReg).ToListAsync();
                    string correo = UsuarioInt[0].correo_usu.ToString();
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usuReg;
                        notifProyecto.seccion = "PLANQUINQUENAL";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = planquinquenalReq.Id.ToString();
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "M";
                        notifProyecto.mensaje = $"Se modificó el plan quinquenal {planquinquenalReq.Descripcion}";
                        notifProyecto.codigo = planquinquenalReq.Id;
                        notifProyecto.modulo = "PQ";

                        var respuestNotif = await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                        dynamic objetoNotif = JsonConvert.DeserializeObject(respuestNotif.ToString());
                        int codigoNotifCreada = int.Parse(objetoNotif.codigoNot.ToString());
                        await _repositoryNotificaciones.EnvioCorreoNotif(camposModificados, correo, "M", "Plan Quinquenal");
                        camposModificados.ForEach(item => item.id = codigoNotifCreada);
                        _context.CorreoTabla.AddRange(camposModificados);
                        _context.SaveChanges();
                    }
                }

                #endregion

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico el proyecto correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar el proyecto"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }

        public async Task<IEnumerable<PlanQuinquenal.Core.Entities.PlanQuinquenal>> Get()
        {
            var pq = await _context.PlanQuinquenal.ToListAsync();
                        
            return pq;
        }

        public async Task<object> CrearComentario(Comentarios_proyecDTO comentario, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.CrearComentario(comentario, idUser, modulo);
            return obj;
        }

        public async Task<object> EliminarComentario(int codigo, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.EliminarComentario(codigo, modulo);
            return obj;
        }

        public async Task<object> CrearDocumento(Docum_proyectoDTO requestDoc, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.CrearDocumento(requestDoc, idUser, modulo);
            dynamic objetoDoc = JsonConvert.DeserializeObject(obj.ToString());
            DocumentoProyRequest archivo = new DocumentoProyRequest
            {
                NombreArchivo = objetoDoc.nombreFinal.ToString(),
                Base64 = requestDoc.Base64
            };
            await CrearDocumentoPr(archivo, modulo);
            return obj;
        }

        public async Task<object> EliminarDocumento(int codigo, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.EliminarDocumento(codigo, modulo);
            return obj;
        }

        public async Task<object> CrearInforme(InformeRequestDTO requestDoc, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.CrearInforme(requestDoc, idUser, modulo);
            return obj;
        }

        public async Task<object> ModificarInforme(InformeRequestDTO requestDoc, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.ModificarInforme(requestDoc, idUser, modulo);
            return obj;
        }

        public async Task<object> EliminarInforme(int codigo, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.EliminarInforme(codigo, modulo);
            return obj;
        }

        public async Task<object> CrearActa(ActaRequestDTO requestDoc, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.CrearActa(requestDoc, idUser, modulo);
            return obj;
        }

        public async Task<object> ModificarActa(ActaRequestDTO requestDoc, int idUser, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.ModificarActa(requestDoc, idUser, modulo);
            return obj;
        }

        public async Task<object> EliminarActa(int codigo, string modulo)
        {
            var obj = await _repositoryMetodosRehusables.EliminarActa(codigo, modulo);
            return obj;
        }

        public static List<CorreoTabla> CompararPropiedades(object valOriginal, object valModificado, string cod_mod, string nomCompleto)
        {
            List<CorreoTabla> camposModificados = new List<CorreoTabla>();
            DateTime fechaActual = DateTime.Today;
            string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
            Type tipo = typeof(object);

            // Obtener las propiedades del tipo
            PropertyInfo[] propiedades = tipo.GetProperties();
            // Comparar las propiedades 
            foreach (PropertyInfo propiedad in propiedades)
            {
                object valor1 = propiedad.GetValue(valOriginal);
                object valor2 = propiedad.GetValue(valModificado);
                string desCampo = "";
                var descriptionAttribute = propiedad.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    desCampo = descriptionAttribute.Description;
                }

                if (!valor1.Equals(valor2))
                {
                    CorreoTabla fila = new CorreoTabla
                    {
                        codigo = cod_mod,
                        campoModificado = desCampo,
                        valorModificado = propiedad.GetValue(valModificado).ToString(),
                        fechaMod = fechaFormateada,
                        usuModif = nomCompleto,

                    };
                    camposModificados.Add(fila);
                }
            }

            return camposModificados;
        }

        public async Task<object> CrearDocumentoPr(DocumentoProyRequest requestDoc, string modulo)
        {
            try
            {
                // Decodificar el base64 a bytes
                byte[] archivoBytes = Convert.FromBase64String(requestDoc.Base64);

                string rutaCompleta = Path.Combine("C:/Ruta/Archivos/", requestDoc.NombreArchivo);

                File.WriteAllBytes(rutaCompleta, archivoBytes);
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el documento correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el documento"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }
    }
}
