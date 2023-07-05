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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ProyectoRepositorio: IProyectoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;

        public ProyectoRepositorio(PlanQuinquenalContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
        }

        public async Task<ProyectoResponseDto> GetById(int id)
        { 

            
            var existe = await _context.Proyecto.Where(x => x.Id == id).FirstOrDefaultAsync();

           
                var proyecto = await _context.Proyecto
                                             .Include(x => x.PlanAnual)
                                             .Include(x => x.PQuinquenal)
                                             .Include(x=>x.Material)
                                             .Include(x => x.Distrito)
                                             .Include(x => x.Constructor)
                                             .Include(x => x.TipoProyecto)
                                             .Include(x => x.EstadoGeneral)
                                             .Include(x => x.TipoRegistro)
                                             .Include(x => x.Baremo)
                                             .Include(x => x.IngenieroResponsable)
                                             .Include(x => x.UsuariosInteresados)
                                             .ThenInclude(x=> x.Usuario)
                                             .Where(x => x.Id == id).FirstOrDefaultAsync();

                var proyectoDto = mapper.Map<ProyectoResponseDto>(proyecto);
                return proyectoDto;
         
             //verificar  si no existe
        }
        public async Task<PaginacionResponseDto<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos)
        {
           
            var queryable =  _context.Proyecto
                                         .Include(x => x.PlanAnual)
                                         .Include(x => x.PQuinquenal)
                                         .Include(x => x.Material)
                                         .Include(x => x.Distrito)
                                         .Include(x => x.Constructor)
                                         .Include(x => x.TipoProyecto)
                                         .Include(x => x.EstadoGeneral)
                                         .Include(x => x.TipoRegistro)
                                         .Include(x => x.IngenieroResponsable)
                                         .Include(x => x.Baremo)
                                         .Where(x => filterProyectos.CodigoProyecto != "" ? x.CodigoProyecto == filterProyectos.CodigoProyecto : true)
                                         .Where(x => filterProyectos.Etapa != 0 ? x.Etapa == filterProyectos.Etapa : true)
                                         .Where(x => filterProyectos.NombreProyecto != "" ? x.descripcion.Contains(filterProyectos.NombreProyecto) : true)
                                         .Where(x => filterProyectos.MaterialId != 0 ? x.Material.Id == filterProyectos.MaterialId : true)
                                         .Where(x => filterProyectos.DistritoId != 0 ? x.Distrito.Id == filterProyectos.DistritoId : true)
                                         .Where(x => filterProyectos.TipoProyectoId != 0 ? x.TipoProyecto.Id == filterProyectos.TipoProyectoId : true)
                                         .Where(x => filterProyectos.PQuinquenalId != 0 ? x.PQuinquenal.Id == filterProyectos.PQuinquenalId : true)
                                         .Where(x => filterProyectos.AñoPq != "" ? x.AñosPQ.Contains(filterProyectos.AñoPq) : true)
                                         .Where(x => filterProyectos.PAnualId != 0 ? x.PlanAnual.Id == filterProyectos.PAnualId : true)
                                         .Where(x => filterProyectos.CodigoMalla != "" ? x.CodigoMalla.Contains(filterProyectos.CodigoMalla) : true)
                                         .Where(x => filterProyectos.ConstructorId != 0 ? x.ConstructorId == filterProyectos.ConstructorId : true)
                                         .Where(x => filterProyectos.IngenieroId != 0 ? x.IngenieroResponsable.cod_usu == filterProyectos.IngenieroId : true)
                                         .Where(x => filterProyectos.UsuarioRegisterId != 0 ? x.UsuarioRegisterId == filterProyectos.UsuarioRegisterId : true)
                                         .AsQueryable();
            int cantidad = queryable.Count();
            var listaPaginada = await queryable.OrderBy(e => e.descripcion).Paginar(filterProyectos).ToListAsync();
            var proyectoDto = mapper.Map<List<ProyectoResponseDto>>(listaPaginada);

            var objeto = new PaginacionResponseDto<ProyectoResponseDto>
            {
                Cantidad = cantidad,
                Model = proyectoDto
            };
            return objeto;

           
        }

        public async Task<ResponseDTO> Add(ProyectoRequestDto proyectoRequestDto, int idUser)
        {
            var existe = await _context.Proyecto.Where(x => x.CodigoProyecto == proyectoRequestDto.CodigoProyecto && x.Etapa ==proyectoRequestDto.Etapa).FirstOrDefaultAsync();

            if (existe != null)
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
                proyecto.PQuinquenalId = proyectoRequestDto.PQuinquenalId==0? null: proyectoRequestDto.PQuinquenalId;
                proyecto.AñosPQ = proyectoRequestDto.AñosPQ;
                proyecto.PlanAnualId = proyectoRequestDto.PlanAnualId == 0? null: proyectoRequestDto.PlanAnualId;
                proyecto.MaterialId = proyectoRequestDto.MaterialId == 0 ? null : proyectoRequestDto.MaterialId;
                proyecto.ConstructorId = proyectoRequestDto.ConstructorId == 0?null : proyectoRequestDto.ConstructorId;
                proyecto.TipoRegistroId = proyectoRequestDto.TipoRegistroId ==0 ? null: proyectoRequestDto.TipoRegistroId;
                proyecto.LongAprobPa = proyectoRequestDto.LongAprobPa;
                proyecto.TipoProyectoId = proyectoRequestDto.TipoProyectoId == 0 ? null : proyectoRequestDto.TipoProyectoId;
                proyecto.BaremoId = proyectoRequestDto.BaremoId == 0 ? null : proyectoRequestDto.BaremoId;
                proyecto.descripcion = "";
                proyecto.Etapa = proyecto.Etapa;
                proyecto.IngenieroResponsableId = null;
                proyecto.EstadoGeneralId = 2;
                proyecto.UsuarioRegisterId = idUser;
                proyecto.UsuarioModificaId = idUser;
                proyecto.FechaGasificacion= null;
                proyecto.FechaRegistro = DateTime.Now;
                proyecto.fechamodifica = DateTime.Now;
                proyecto.LongImpedimentos = 0;
                proyecto.LongRealHab = 0;
                proyecto.LongRealPend = 0;
                proyecto.LongProyectos = 0;
                _context.Add(proyecto);
                await _context.SaveChangesAsync();

                if (proyectoRequestDto.UsuariosInteresados.Count > 0)
                {
                    List<UsuariosInteresadosPy> listPqUser = new List<UsuariosInteresadosPy>();
                    foreach (var item in proyectoRequestDto.UsuariosInteresados)
                    {
                        var existeUsu = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                        if(existeUsu != null)
                        {
                            UsuariosInteresadosPy pqUser = new UsuariosInteresadosPy();
                            pqUser.ProyectoId = proyecto.Id;
                            pqUser.UsuarioId = item;
                            pqUser.Estado = true;
                            listPqUser.Add(pqUser);
                        }
                       
                    }
                    _context.AddRange(listPqUser);
                    await _context.SaveChangesAsync();
                }

                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.CreacionExistosa
                };
            }
        }

        public async Task<ResponseDTO> Update(ProyectoRequestUpdateDto p, int id, int idUser)
        {
            try
            {
                
                var existeN = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(p.CodigoProyecto) && x.Etapa.Equals(p.Etapa) && x.Id != id).FirstOrDefaultAsync();

                if (existeN == null)
                {
                    var existe = await _context.Proyecto.Where(x => x.Id == id).FirstOrDefaultAsync();

                    if (existeN == null)
                    {
                        existe.descripcion = p.Descripcion;
                        existe.PQuinquenalId = p.PQuinquenalId;
                        existe.AñosPQ = p.AñosPQ;
                        existe.PlanAnualId=p.PlanAnualId==null || p.PlanAnualId==0 ? null : p.PlanAnualId;
                        existe.MaterialId = p.MaterialId == null || p.MaterialId == 0 ? null : p.MaterialId;
                        existe.DistritoId = p.DistritoId == null || p.DistritoId == 0 ? null : p.DistritoId;
                        existe.TipoProyectoId = p.TipoProyectoId == null || p.TipoProyectoId == 0 ? null : p.TipoProyectoId;
                        existe.Etapa = p.Etapa;
                        existe.CodigoMalla = p.CodigoMalla;
                        existe.TipoRegistroId = p.TipoRegistroId == null || p.TipoRegistroId == 0 ? null : p.TipoRegistroId;
                        existe.IngenieroResponsableId = p.IngenieroResponsableId == null || p.IngenieroResponsableId == 0 ? null: p.IngenieroResponsableId;
                        existe.ConstructorId = p.ConstructorId == null || p.ConstructorId == 0 ? null : p.ConstructorId;
                        existe.BaremoId = p.BaremoId == null || p.BaremoId == 0 ? null : p.BaremoId;
                        existe.FechaGasificacion = p.FechaGacificacion != null || p.FechaGacificacion == "" ? DateTime.Parse(p.FechaGacificacion):null;
                        existe.LongAprobPa = p.LongAprobPa;
                        existe.LongRealHab = p.LongRealHab;
                        existe.LongProyectos = p.LongProyectos;
                        existe.UsuarioModificaId = idUser;
                        existe.fechamodifica = DateTime.Now;
                        _context.Update(existe);
                        await _context.SaveChangesAsync();

                        if (p.UsuariosInteresados.Count > 0)
                        {
                            var userInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == id).ToListAsync();

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
                                    pqUser.ProyectoId = id;
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
                else
                {
                    var objeto = new ResponseDTO
                    {
                        Message = "Ya existe el proyecto con esa etapa",
                        Valid = false
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
    }
}
