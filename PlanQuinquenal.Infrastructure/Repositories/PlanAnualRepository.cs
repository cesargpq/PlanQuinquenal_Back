using AutoMapper;
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
    public class PlanAnualRepository : IPlanAnualRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly ITrazabilidadRepository _trazabilidadRepository;

        public PlanAnualRepository(PlanQuinquenalContext context, IMapper mapper, ITrazabilidadRepository trazabilidadRepository)
        {
            this._context = context;
            this.mapper = mapper;
            this._trazabilidadRepository = trazabilidadRepository;
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
       
        public async Task<ResponseDTO> Add(PQuinquenalReqDTO p,DatosUsuario usuario)
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
                quinquenal.UsuarioRegisterId = usuario.UsuaroId;
                quinquenal.UsuarioModifica = usuario.UsuaroId;
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


                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO PlanAnual , Crear").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "PlanAnual";
                    trazabilidad.Evento = "Crear";
                    trazabilidad.DescripcionEvento = $"Se creó correctamente el Plan Anual {quinquenal.AnioPlan} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
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

        
        public async Task<ResponseDTO> Update(UpdatePlanQuinquenalDto dto, int id, DatosUsuario usuario)
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
                    pqUpdate.UsuarioModifica = usuario.UsuaroId;
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


                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO PlanAnual , Editar").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "PlanAnual";
                        trazabilidad.Evento = "Editar";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el Plan Anual {pqUpdate.AnioPlan} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
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
