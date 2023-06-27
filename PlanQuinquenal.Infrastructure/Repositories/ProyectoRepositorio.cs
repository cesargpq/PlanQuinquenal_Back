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
            var existe = await _context.Proyecto.Where(x => x.CodigoProyecto == proyectoRequestDto.CodigoProyecto).FirstOrDefaultAsync();

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
                var proyecto = mapper.Map<Proyecto>(proyectoRequestDto);
                proyecto.descripcion = "";
                proyecto.Etapa = 0;
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
                proyecto.LongReemplazada = 0;
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

        
    }
}
