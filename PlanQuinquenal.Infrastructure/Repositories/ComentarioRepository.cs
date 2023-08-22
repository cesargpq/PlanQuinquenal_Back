using ApiDavis.Core.Utilidades;
using AutoMapper;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs;
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
using static iTextSharp.text.pdf.AcroFields;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public ComentarioRepository(PlanQuinquenalContext context, IMapper mapper, ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones)
        {
            this._context = context;
            this.mapper = mapper;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }
        public async Task<ResponseDTO> Add(ComentarioRequestDTO c, DatosUsuario usuario)
        {

            try
            {
                var obj = new ResponseDTO();
                var tipoSeguimiento = await _context.TipoSeguimiento.Where(x => x.Id == c.TipoSeguimientoId).FirstOrDefaultAsync();
                string comentarioregister = "";
                string idComentario = "";
                int codigo = 0;
                string nomTipo = "";
                
                if (tipoSeguimiento.Descripcion.Equals("Proyectos"))
                {
                    ComentarioPY comentario = new ComentarioPY();
                    comentario.Descripcion = c.Descripcion;
                    comentario.TipoSeguimientoId = c.TipoSeguimientoId;
                    comentario.CodigoProyecto = c.CodigoProyecto;
                    comentario.TipoComentario = c.TipoComentario;
                    comentario.UsuarioId = usuario.UsuaroId;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
                    comentarioregister = c.Descripcion;
                    idComentario = comentario.CodigoProyecto;
                    codigo = comentario.Id;
                    nomTipo = "Proyecto";
                }
                else if (tipoSeguimiento.Descripcion.Equals("Plan Quinquenal"))
                {
                    var comentario = mapper.Map<COMENTARIOPQ>(c);
                    comentario.PQuinquenalId = c.ProyectoId;
                    comentario.UsuarioId = usuario.UsuaroId;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
                    comentarioregister = c.Descripcion;
                    var py = await _context.PQuinquenal.Where(x => x.Id.Equals(c.ProyectoId)).FirstOrDefaultAsync();
                    idComentario = py.AnioPlan;
                    codigo = comentario.Id;
                    nomTipo = "Plan Quinquenal";
                }
                else if (tipoSeguimiento.Descripcion.Equals("Plan Anual"))
                {
                    var comentario = mapper.Map<COMENTARIOPA>(c);
                    comentario.PlanAnualId = c.ProyectoId;
                    comentario.UsuarioId = usuario.UsuaroId;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
                    comentarioregister = c.Descripcion;
                    var py = await _context.PlanAnual.Where(x => x.Id.Equals(c.ProyectoId)).FirstOrDefaultAsync();
                    idComentario = py.AnioPlan;
                    codigo = comentario.Id;
                    nomTipo = "Plan Anual";
                }
                else if (tipoSeguimiento.Descripcion.Equals("Gestión de Reemplazo"))
                {
                    var comentario = mapper.Map<COMENTARIOBR>(c);
                    comentario.BolsaReemplazoId = c.ProyectoId;
                    comentario.UsuarioId = usuario.UsuaroId;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
                    var py = await _context.BolsaReemplazo.Where(x => x.Id.Equals(c.ProyectoId)).FirstOrDefaultAsync();
                    idComentario = py.CodigoProyecto;
                    comentarioregister = c.Descripcion;
                    codigo = comentario.Id;
                    nomTipo = "Bolsa Reemplazo";
                }
                else
                {
                    obj.Message = Constantes.BusquedaNoExitosa;
                    obj.Valid = true;
                }
                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Comentario , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Comentario";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creo el comentario satisfactoriamente {comentarioregister} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }

                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = idComentario
                    };

                    #region Envio de notificacion
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x=>x.estado_user == "A").Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regCom == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Comentario";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = idComentario;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el comentario en {nomTipo} : {idComentario}";
                            notifProyecto.codigo = codigo;
                            notifProyecto.modulo = "C";

                            await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Comentario");
                        }
                    }

                    #endregion


                    //#region Comparacion de estructuras y agregacion de cambios
                    //var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    //string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    //string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    //List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    //var proyectosearch = await _context.Proyecto.Where(x => x.Id == c.ProyectoId).FirstOrDefaultAsync();
                    //var usuInt = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto == c.CodigoProyecto).ToListAsync();
                    //CorreoTabla correoDatos = new CorreoTabla
                    //{
                    //    codigo = proyectosearch != null ?proyectosearch.CodigoProyecto:null
                    //};

                    //composCorreo.Add(correoDatos);
                    //#endregion

                    //#region Envio de notificacion

                    //foreach (var listaUsuInters in usuInt)
                    //{
                    //    int cod_usu = listaUsuInters.UsuarioId;
                    //    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPry == true).ToListAsync();
                    //    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                    //    string correo = UsuarioInt[0].correo_usu.ToString();
                    //    if (lstpermisos.Count() == 1)
                    //    {
                    //        Notificaciones notifProyecto = new Notificaciones();
                    //        notifProyecto.cod_usu = cod_usu;
                    //        notifProyecto.seccion = "PROYECTOS";
                    //        notifProyecto.nombreComp_usu = NomCompleto;
                    //        notifProyecto.cod_reg = proyectosearch.CodigoProyecto;
                    //        notifProyecto.area = nomPerfil;
                    //        notifProyecto.fechora_not = DateTime.Now;
                    //        notifProyecto.flag_visto = false;
                    //        notifProyecto.tipo_accion = "C";
                    //        notifProyecto.mensaje = $"Se creó el comentario en el proyecto {proyectosearch.CodigoProyecto}";
                    //        notifProyecto.codigo = proyectosearch.Id;
                    //        notifProyecto.modulo = "P";

                    //        await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                    //        await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Proyectos");
                    //    }
                    //}

                    //#endregion



                
                

                return obj;

            }
            catch (Exception e)
            {

                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.ErrorSistema
                };
            }
        }
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPA(RequestComentarioDTO p, int idUser)
        {

            var resultado =  _context.COMENTARIOPA
                                    .Include(x => x.Usuario)
                                    .Where(x => x.PlanAnualId == p.Codigo)
                                    .OrderByDescending(x=>x.Fecha)
                                    .AsQueryable();


            var usuarioLog = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();

            try
            {
                var usuarioInt = await _context.UsuariosInteresadosPQ.Where(x => x.PQuinquenalId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
                List<COMENTARIOPA> lista = new List<COMENTARIOPA>();

                foreach (var comentario in resultado)
                {
                    if (comentario.TipoComentario == 1)
                    {
                        if (usuarioInt != null || usuarioLog.Perfil.nombre_perfil == "Administrador" || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                    else
                    {
                        var usuario = await _context.Usuario.Where(x => x.cod_usu == comentario.UsuarioId).FirstOrDefaultAsync();
                        if (comentario.Usuario.Unidad_negociocod_und == usuarioLog.Unidad_negociocod_und || usuarioInt != null || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                }

                var elementosPagina = lista.Skip((p.Pagina - 1) * p.RecordsPorPagina).Take(p.RecordsPorPagina);

                List<ComentarioResultDTO> listaComent = new List<ComentarioResultDTO>();
                foreach (var item in elementosPagina)
                {
                    var ComentarioRsult = new ComentarioResultDTO
                    {
                        usuario = item.Usuario.nombre_usu + " " + item.Usuario.apellido_usu,
                        perfil = item.Usuario.Perfil.nombre_perfil,
                        Descripcion = item.Descripcion,
                        Fecha = item.Fecha
                    };
                    listaComent.Add(ComentarioRsult);
                }

                var map = new PaginacionResponseDtoException<ComentarioResultDTO>
                {
                    Cantidad = lista.Count,
                    Model = listaComent
                };
                return map;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPQ(RequestComentarioDTO p, int idUser)
        {
            
            var resultado =  _context.COMENTARIOPQ
                                    .Include(x => x.Usuario)
                                    .ThenInclude(y=>y.Perfil)
                                    .Where(x => x.PQuinquenalId == p.Codigo)
                                    .OrderByDescending(x => x.Fecha)
                                    .AsQueryable();



            var usuarioLog = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();

            try
            {
                var usuarioInt = await _context.UsuariosInteresadosPQ.Where(x => x.PQuinquenalId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
                List<COMENTARIOPQ> lista = new List<COMENTARIOPQ>();

                foreach (var comentario in resultado)
                {
                    if (comentario.TipoComentario == 1)
                    {
                        if (usuarioInt != null || usuarioLog.Perfil.nombre_perfil == "Administrador" || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                    else
                    {
                        var usuario = await _context.Usuario.Where(x => x.cod_usu == comentario.UsuarioId).FirstOrDefaultAsync();
                        if (comentario.Usuario.Unidad_negociocod_und == usuarioLog.Unidad_negociocod_und || usuarioInt != null || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                }

                var elementosPagina = lista.Skip((p.Pagina - 1) * p.RecordsPorPagina).Take(p.RecordsPorPagina);

                List<ComentarioResultDTO> listaComent = new List<ComentarioResultDTO>();
                foreach (var item in elementosPagina)
                {
                    var ComentarioRsult = new ComentarioResultDTO
                    {
                        usuario = item.Usuario.nombre_usu + " " + item.Usuario.apellido_usu,
                        perfil = item.Usuario.Perfil.nombre_perfil,
                        Descripcion = item.Descripcion,
                        Fecha = item.Fecha
                    };
                    listaComent.Add(ComentarioRsult);
                }

                var map = new PaginacionResponseDtoException<ComentarioResultDTO>
                {
                    Cantidad = lista.Count,
                    Model = listaComent
                };
                return map;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarBR(RequestComentarioDTO p, int idUser)
        {

            var resultado =  _context.COMENTARIOBR
                                    .Include(x => x.Usuario)
                                    .ThenInclude(y => y.Perfil)
                                    .Where(x => x.BolsaReemplazoId == p.Codigo)
                                    .OrderByDescending(x => x.Fecha)
                                    .AsQueryable();

           
            var elementosPagina =  resultado.Skip((p.Pagina - 1) * p.RecordsPorPagina).Take(p.RecordsPorPagina);

            List<ComentarioResultDTO> listaComent = new List<ComentarioResultDTO>();
            foreach (var item in elementosPagina)
            {
                var perfil = await _context.Perfil.Where(x => x.cod_perfil == item.Usuario.Perfilcod_perfil).FirstOrDefaultAsync();
                var ComentarioRsult = new ComentarioResultDTO
                {
                    usuario = item.Usuario.nombre_usu + " " + item.Usuario.apellido_usu,

                    perfil = perfil.nombre_perfil,
                    Descripcion = item.Descripcion,
                    Fecha = item.Fecha
                };
                listaComent.Add(ComentarioRsult);
            }
            
            var map = new PaginacionResponseDtoException<ComentarioResultDTO>
            {
                Cantidad = resultado.Count(),
                Model = listaComent
            };
            return map;
        }
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPY(RequestComentarioDTO p, int idUser)
        {
            
            var resultado = await _context.ComentarioPY
                                    .Include(x=>x.Usuario)
                                    .Where(x=> x.CodigoProyecto ==p.CodigoProyecto)
                                    .OrderByDescending(x=>x.Fecha)
                                    .ToListAsync();

            var usuarioLog = await _context.Usuario.Include(x=>x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();

            try
            {
                var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto.Equals(p.CodigoProyecto) && x.UsuarioId == idUser).FirstOrDefaultAsync();
                List<ComentarioPY> lista = new List<ComentarioPY>();

                foreach (var comentario in resultado)
                {
                    if (comentario.TipoComentario == 1)
                    {
                        if (usuarioInt != null || usuarioLog.Perfil.nombre_perfil == "Administrador" || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                    else
                    {
                        var usuario = await _context.Usuario.Where(x => x.cod_usu == comentario.UsuarioId).FirstOrDefaultAsync();
                        if (comentario.Usuario.Unidad_negociocod_und == usuarioLog.Unidad_negociocod_und || usuarioInt != null || idUser == comentario.UsuarioId)
                        {
                            lista.Add(comentario);
                        }
                    }
                }

                var elementosPagina = lista.Skip((p.Pagina - 1) * p.RecordsPorPagina).Take(p.RecordsPorPagina);

                List<ComentarioResultDTO> listaComent = new List<ComentarioResultDTO>();
                foreach (var item in elementosPagina)
                {
                    var ComentarioRsult = new ComentarioResultDTO
                    {
                        usuario = item.Usuario.nombre_usu + " " + item.Usuario.apellido_usu,
                        perfil = item.Usuario.Perfil.nombre_perfil,
                        Descripcion = item.Descripcion,
                        Fecha = item.Fecha
                    };
                    listaComent.Add(ComentarioRsult);
                }

                var map = new PaginacionResponseDtoException<ComentarioResultDTO>
                {
                    Cantidad = lista.Count,
                    Model = listaComent
                };
                return map;
            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }
    }
}
