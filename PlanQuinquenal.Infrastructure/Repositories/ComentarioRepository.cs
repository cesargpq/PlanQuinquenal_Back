﻿using ApiDavis.Core.Utilidades;
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

                if (tipoSeguimiento.Descripcion.Equals("Proyectos"))
                {
                    var comentario = mapper.Map<ComentarioPY>(c);

                    comentario.UsuarioId = usuario.UsuaroId;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
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


                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Comentario , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Comentario";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creo el comentario satisfactoriamente {comentario.Descripcion} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }


                    #region Comparacion de estructuras y agregacion de cambios
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    var proyectosearch = await _context.Proyecto.Where(x => x.Id == c.ProyectoId).FirstOrDefaultAsync();
                    var usuInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == c.ProyectoId).ToListAsync();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = proyectosearch != null ?proyectosearch.CodigoProyecto:null
                    };

                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion

                    foreach (var listaUsuInters in usuInt)
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
                            notifProyecto.cod_reg = proyectosearch.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el comentario en el proyecto {proyectosearch.CodigoProyecto}";
                            notifProyecto.codigo = proyectosearch.Id;
                            notifProyecto.modulo = "P";

                            await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Proyectos");
                        }
                    }

                    #endregion



                }
                else
                {
                    obj.Message = Constantes.BusquedaNoExitosa;
                    obj.Valid = true;
                }
                

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

            var resultado = await _context.COMENTARIOPA
                                    .Include(x => x.Usuario)
                                    .Where(x => x.PlanAnualId == p.Codigo)
                                    .OrderByDescending(x=>x.Fecha)
                                    .ToListAsync();

            var usuarioLog = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();
            var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
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
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPQ(RequestComentarioDTO p, int idUser)
        {

            var resultado = await _context.COMENTARIOPQ
                                    .Include(x => x.Usuario)
                                    .Where(x => x.PQuinquenalId == p.Codigo)
                                    .OrderByDescending(x => x.Fecha)
                                    .ToListAsync();

            var usuarioLog = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();
            var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
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
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarBR(RequestComentarioDTO p, int idUser)
        {

            var resultado = await _context.COMENTARIOBR
                                    .Include(x => x.Usuario)
                                    .Where(x => x.BolsaReemplazoId == p.Codigo)
                                    .OrderByDescending(x => x.Fecha)
                                    .ToListAsync();

            var usuarioLog = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();
            var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
            List<COMENTARIOBR> lista = new List<COMENTARIOBR>();

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
        public async Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPY(RequestComentarioDTO p, int idUser)
        {
            
            var resultado = await _context.ComentarioPY
                                    .Include(x=>x.Usuario)
                                    .Where(x=> x.ProyectoId ==p.Codigo)
                                    .OrderByDescending(x=>x.Fecha)
                                    .ToListAsync();

            var usuarioLog = await _context.Usuario.Include(x=>x.Perfil).Where(x => x.cod_usu == idUser).FirstOrDefaultAsync();
            var usuarioInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == p.Codigo && x.UsuarioId == idUser).FirstOrDefaultAsync();
            List<ComentarioPY> lista = new List<ComentarioPY>();

            foreach (var comentario in resultado)
            {
                if(comentario.TipoComentario == 1)
                {
                    if (usuarioInt != null || usuarioLog.Perfil.nombre_perfil=="Administrador" || idUser == comentario.UsuarioId )
                    {
                        lista.Add(comentario);
                    }
                }
                else
                {
                    var usuario = await _context.Usuario.Where(x => x.cod_usu == comentario.UsuarioId).FirstOrDefaultAsync();
                    if(comentario.Usuario.Unidad_negociocod_und == usuarioLog.Unidad_negociocod_und || usuarioInt != null || idUser == comentario.UsuarioId)
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
                    Descripcion =item.Descripcion,
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
    }
}
