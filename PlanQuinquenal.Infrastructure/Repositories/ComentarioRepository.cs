using ApiDavis.Core.Utilidades;
using AutoMapper;
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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;

        public ComentarioRepository(PlanQuinquenalContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
        }
        public async Task<ResponseDTO> Add(ComentarioRequestDTO c, int idUser)
        {

            try
            {
                var obj = new ResponseDTO();
                var tipoSeguimiento = await _context.TipoSeguimiento.Where(x => x.Id == c.TipoSeguimientoId).FirstOrDefaultAsync();

                if (tipoSeguimiento.Descripcion.Equals("Proyectos"))
                {
                    var comentario = mapper.Map<ComentarioPY>(c);

                    comentario.UsuarioId = idUser;
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
                    comentario.UsuarioId = idUser;
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
                    comentario.UsuarioId = idUser;
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
                    comentario.UsuarioId = idUser;
                    comentario.Fecha = DateTime.Now;
                    _context.Add(comentario);
                    await _context.SaveChangesAsync();
                    obj.Message = Constantes.CreacionExistosa;
                    obj.Valid = true;
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
                    usuario = elementosPagina.ElementAt(0).Usuario.nombre_usu + " " + elementosPagina.ElementAt(0).Usuario.apellido_usu,
                    perfil = elementosPagina.ElementAt(0).Usuario.Perfil.nombre_perfil,
                    Descripcion = elementosPagina.ElementAt(0).Descripcion,
                    Fecha = elementosPagina.ElementAt(0).Fecha
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
                    usuario = elementosPagina.ElementAt(0).Usuario.nombre_usu + " " + elementosPagina.ElementAt(0).Usuario.apellido_usu,
                    perfil = elementosPagina.ElementAt(0).Usuario.Perfil.nombre_perfil,
                    Descripcion = elementosPagina.ElementAt(0).Descripcion,
                    Fecha = elementosPagina.ElementAt(0).Fecha
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
                    usuario = elementosPagina.ElementAt(0).Usuario.nombre_usu + " " + elementosPagina.ElementAt(0).Usuario.apellido_usu,
                    perfil = elementosPagina.ElementAt(0).Usuario.Perfil.nombre_perfil,
                    Descripcion = elementosPagina.ElementAt(0).Descripcion,
                    Fecha = elementosPagina.ElementAt(0).Fecha
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
                    usuario = elementosPagina.ElementAt(0).Usuario.nombre_usu + " " + elementosPagina.ElementAt(0).Usuario.apellido_usu,
                    perfil = elementosPagina.ElementAt(0).Usuario.Perfil.nombre_perfil,
                    Descripcion = elementosPagina.ElementAt(0).Descripcion,
                    Fecha = elementosPagina.ElementAt(0).Fecha
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
