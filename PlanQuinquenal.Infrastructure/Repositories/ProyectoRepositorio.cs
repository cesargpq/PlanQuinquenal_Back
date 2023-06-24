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
                                             .Include(x => x.IngenieroResponsable)
                                             .Where(x => x.Id == id).FirstOrDefaultAsync();

                var proyectoDto = mapper.Map<ProyectoResponseDto>(proyecto);
                return proyectoDto;
         
             //verificar  si no existe
        }
        public async Task<List<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos)
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
            var listaPaginada = await queryable.OrderBy(e => e.descripcion).Paginar(filterProyectos).ToListAsync();
            var proyectoDto = mapper.Map<List<ProyectoResponseDto>>(listaPaginada);

          
                               
            return proyectoDto;
        }

    }
}
