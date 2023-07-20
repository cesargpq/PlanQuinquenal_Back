using AutoMapper;
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
    public class PlanAnualRepository : IPlanAnualRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;

        public PlanAnualRepository(PlanQuinquenalContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
        }

        public async Task<PaginacionResponseDtoException<PQuinquenalResponseDto>> GetAll(PQuinquenalRequestDTO p)
        {
            try
            {
                if (p.PQ.Equals("")) p.PQ = null;
                if (p.Aprobaciones.Equals("")) p.Aprobaciones = null;
                if (p.Descripcion.Equals("")) p.Descripcion = null;
                if (p.FechaRegistro.Equals("")) p.FechaRegistro = null;
                if (p.FechaModificacion.Equals("")) p.FechaModificacion = null;
                if (p.EstadoAprobacion == 0) p.EstadoAprobacion = null;
                if (p.UsuarioRegister == 0) p.UsuarioRegister = null;
                if (p.UsuarioModifica == 0) p.UsuarioModifica = null;

                var resultad = await _context.PQuinquenalResponseDto.FromSqlInterpolated($"EXEC plananuallistar {p.PQ} , {p.Aprobaciones} , {p.EstadoAprobacion} , {p.Descripcion} , {p.FechaRegistro} , {p.FechaModificacion} , {p.UsuarioRegister} , {p.UsuarioModifica} , {p.Pagina} , {p.RecordsPorPagina}").ToListAsync();

                var lista = new PaginacionResponseDtoException<PQuinquenalResponseDto>
                {
                    Cantidad = resultad.Count() == 0 ? 0 : resultad.ElementAt(0).total,
                    Model = resultad
                };
                return lista;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public async Task<PaginacionResponseDtoException<PQuinquenalResponseDto>> GetAll(PQuinquenalRequestDTO p)
        //{
        //    var queryable = await _context.PlanAnual
        //                                    .Include(x => x.EstadoAprobacion)
        //                                    .Include(x => x.Proyecto)
        //                                    .ToListAsync();
        //    List<PQuinquenalResponseDto> listaF = new List<PQuinquenalResponseDto>();
        //    foreach (var item in queryable)
        //    {
        //        PQuinquenalResponseDto obj = new PQuinquenalResponseDto();
        //        obj.Id = item.Id;
        //        obj.AnioPlan = item.AnioPlan;
        //        obj.Descripcion = item.Descripcion;
        //        obj.EstadoAprobacionId = item.EstadoAprobacionId;
        //        obj.EstadoAprobacion = item.EstadoAprobacion.Descripcion;
        //        //obj.FechaAprobacion = item.FechaAprobacion.ToString("dd/MM/yyyy");
        //        //obj.FechaModifica = item.FechaModifica.ToString("dd/MM/yyyy");
        //        //obj.FechaRegistro = item.FechaRegistro.ToString("dd/MM/yyyy");
        //        var userC = await _context.Usuario.Where(x => x.cod_usu == item.UsuarioRegisterId).FirstOrDefaultAsync();
        //        var userM = await _context.Usuario.Where(x => x.cod_usu == item.UsuarioModifica).FirstOrDefaultAsync();
        //        obj.UsuarioModifica = userM.nombre_usu + " " + userM.apellido_usu;
        //        obj.UsuarioRegister = userC.nombre_usu + " " + userC.apellido_usu;
        //        obj.UsuarioModificaId = userM.cod_usu;
        //        obj.UsuarioRegisterId = userC.cod_usu;
        //        var proyectos = await _context.Proyecto.Where(x => x.PlanAnualId == item.Id).ToListAsync();
        //        decimal dato = 0;
        //        foreach (var pry in proyectos)
        //        {
        //            if (pry.LongAprobPa > 0)
        //                dato += pry.LongRealHab / pry.LongAprobPa;
        //        }
        //        if (proyectos.Count() > 0)
        //        {
        //            obj.Avance = dato / proyectos.Count();
        //        }
        //        else
        //        {
        //            obj.Avance = 0;
        //        }

        //        listaF.Add(obj);
        //    }
        //    var listaFin = listaF
        //                        .Where(x => p.PQ != "" ? x.AnioPlan.Contains(p.PQ) : true)
        //                        .Where(x => p.Aprobaciones != "" ? x.FechaAprobacion.Equals(p.Aprobaciones.Replace("-", "/")) : true)
        //                        .Where(x => p.EstadoAprobacion != 0 ? x.EstadoAprobacionId == p.EstadoAprobacion : true)
        //                        .Where(x => p.Descripcion != "" ? x.Descripcion.Contains(p.Descripcion) : true)
        //                        .Where(x => p.FechaRegistro != "" ? x.FechaRegistro.Equals(p.FechaRegistro.Replace("-", "/")) : true)
        //                        .Where(x => p.FechaModificacion != "" ? x.FechaModifica.Equals(p.FechaModificacion.Replace("-", "/")) : true)
        //                        .Where(x => p.UsuarioRegister != 0 ? x.UsuarioRegisterId == p.UsuarioRegister : true)
        //                        .Where(x => p.UsuarioModifica != 0 ? x.UsuarioModificaId == p.UsuarioModifica : true)
        //                        //.Where(x => p.PorcentajeAvance == 1 && p.valor != 0 ? x.Avance < Convert.ToDecimal(p.valor) / 100 :
        //                        //p.PorcentajeAvance == 2 && p.valor != 0 ? x.Avance <= Convert.ToDecimal(p.valor) / 100 :
        //                        //p.PorcentajeAvance == 3 && p.valor != 0 ? x.Avance > Convert.ToDecimal(p.valor) / 100 :
        //                        //p.PorcentajeAvance == 4 && p.valor != 0 ? x.Avance >= Convert.ToDecimal(p.valor) / 100 :
        //                        //p.PorcentajeAvance == 5 && p.valor != 0 ? x.Avance == Convert.ToDecimal(p.valor) / 100 :
        //                        //p.PorcentajeAvance == 6 && p.valor != 0 ? x.Avance != Convert.ToDecimal(p.valor) / 100 :
        //                        //true)
        //                        .AsQueryable();

        //    var fin = listaFin.Skip((p.Pagina - 1) * p.RecordsPorPagina).Take(p.RecordsPorPagina);

        //    var lista = new PaginacionResponseDtoException<PQuinquenalResponseDto>
        //    {
        //        Cantidad = listaFin.Count(),
        //        Model = fin
        //    };
        //    return lista;
        //}
        public async Task<ResponseDTO> Add(PQuinquenalReqDTO p, int id)
        {
            try
            {
                var existe = await _context.PlanAnual.Where(x => x.AnioPlan.Equals(p.AnioPlan)).FirstOrDefaultAsync();
                if (existe != null)
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.ExisteRegistro
                    };
                }
                var quinquenal = mapper.Map<PlanAnual>(p);

                quinquenal.FechaRegistro = DateTime.Now;
                quinquenal.FechaModifica = DateTime.Now;
                quinquenal.UsuarioRegisterId = id;
                quinquenal.UsuarioModifica = id;
                quinquenal.Estado = true;

                _context.Add(quinquenal);
                await _context.SaveChangesAsync();

                if (p.UsuariosInteresados.Count > 0)
                {
                    foreach (var item in p.UsuariosInteresados)
                    {
                        UsuariosInteresadosPA obj = new UsuariosInteresadosPA();
                        obj.PlanAnualId = quinquenal.Id;
                        obj.UsuarioId = item;
                        obj.estado = true;
                        _context.Add(obj);
                        await _context.SaveChangesAsync();
                    }
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
                    Valid = true,
                    Message = Constantes.ErrorSistema
                };
            }
        }

        
        public async Task<ResponseDTO> Update(UpdatePlanQuinquenalDto dto, int id, int idUser)
        {
            try
            {
                var pq = await _context.PlanAnual.Where(x => x.Id != id && x.AnioPlan.Equals(dto.AnioPlan)).FirstOrDefaultAsync();

                if (pq == null)
                {
                    var pqUpdate = await _context.PlanAnual.Where(x => x.Id == id).FirstOrDefaultAsync();
                    pqUpdate.AnioPlan = dto.AnioPlan;
                    pqUpdate.Descripcion = dto.Descripcion;
                    pqUpdate.EstadoAprobacionId = dto.EstadoAprobacionId;
                    pqUpdate.FechaAprobacion = Convert.ToDateTime(dto.FechaAprobacion);
                    pqUpdate.FechaModifica = DateTime.Now;
                    pqUpdate.UsuarioModifica = idUser;
                    _context.Update(pqUpdate);
                    await _context.SaveChangesAsync();

                    if (dto.UsuariosInteresados.Count() > 0)
                    {
                        var userInt = await _context.UsuariosInteresadosPA.Where(x => x.PlanAnualId == pqUpdate.Id).ToListAsync();
                        foreach (var item in userInt)
                        {
                            _context.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                        foreach (var item in dto.UsuariosInteresados)
                        {
                            UsuariosInteresadosPA usr = new UsuariosInteresadosPA();
                            usr.PlanAnualId = pqUpdate.Id;
                            usr.UsuarioId = item;
                            usr.estado = true;
                            _context.Add(usr);
                            await _context.SaveChangesAsync();
                        }
                    }
                    var result = new ResponseDTO
                    {
                        Message = Constantes.ActualizacionSatisfactoria,
                        Valid = true
                    };
                    return result;
                }
                else
                {
                    var result = new ResponseDTO
                    {
                        Message = Constantes.ExisteRegistro,
                        Valid = false
                    };
                    return result;
                }
            }
            catch (Exception)
            {

                var result = new ResponseDTO
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false
                };
                return result;
            }
        }
        public async Task<ResponseEntidadDto<PQuinquenalResponseDto>> GetById(int Id)
        {
            try
            {

                var resultad = await _context.PQuinquenalResponseDto.FromSqlRaw($"EXEC plananualbyid  {Id}").ToListAsync();


                if (resultad.Count > 0)
                {
                    var result = new ResponseEntidadDto<PQuinquenalResponseDto>
                    {
                        Message = Constantes.BusquedaExitosa,
                        Valid = true,
                        Model = resultad[0]
                    };
                    return result;
                }
                else
                {
                    var result = new ResponseEntidadDto<PQuinquenalResponseDto>
                    {
                        Message = Constantes.BusquedaNoExitosa,
                        Valid = false,
                        Model = null
                    };
                    return result;
                }





            }
            catch (Exception e)
            {

                var result = new ResponseEntidadDto<PQuinquenalResponseDto>
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false,
                    Model = null
                };
                return result;
            }
        }

        
    }
}
