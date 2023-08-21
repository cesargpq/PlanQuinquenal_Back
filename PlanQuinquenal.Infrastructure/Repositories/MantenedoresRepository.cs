using ApiDavis.Core.Utilidades;
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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class MantenedoresRepository : IRepositoryMantenedores
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;

        public MantenedoresRepository(PlanQuinquenalContext context, IMapper mapper) 
        {
            _context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<MaestroResponseDto>> GetAllByAttribute(string attribute)
        {
            List<MaestroResponseDto> dto = new List<MaestroResponseDto>();

            if (attribute.ToUpper().Equals("Material".ToUpper()))
            {
                var dato = await _context.Material.Where(x=>x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            if (attribute.ToUpper().Equals("ValidacionLegal".ToUpper()))
            {
                var dato = await _context.ValidacionLegal.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            
            if (attribute.ToUpper().Equals("Reemplazo".ToUpper()))
            {
                var dato = await _context.Reemplazo.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("Baremo")){
                var dato = await _context.Baremo.Where(x => x.Estado == true).ToListAsync();

                foreach (var item in dato)
                {
                    MaestroResponseDto obj = new MaestroResponseDto();
                    obj.Descripcion = item.CodigoBaremo;
                    obj.Id=item.Id;
                    dto.Add(obj);
                }
                
               
            }
            else if (attribute.Equals("ZonaPermiso"))
            {
                var dato = await _context.ZonaPermiso.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("TipoInforme"))
            {
                var dato = await _context.TipoInforme.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("TipoSeguimiento"))
            {
                var dato = await _context.TipoSeguimiento.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("TipoPermisosProyecto"))
            {
                var dato = await _context.TipoPermisosProyecto.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("EstadoPermisos"))
            {
                var dato = await _context.EstadoPermisos.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("EstadoPermisos"))
            {
                var dato = await _context.EstadoPermisos.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }else if (attribute.Equals("Usuario"))
            {
                var dato = await _context.Usuario.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("TipoUsuario"))
            {
                var dato = await _context.TipoUsuario.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("ProblematicaReal"))
            {
                var dato = await _context.ProblematicaReal.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("CausalReemplazo"))
            {
                var dato = await _context.CausalReemplazo.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }

            else if (attribute.Equals("EstadoPQ"))
            {
                var dato = await _context.EstadoAprobacion.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }

            else if (attribute.Equals("TipoImpedimento"))
            {
                var dato = await _context.TipoImpedimento.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }

            else if (attribute.Equals("Distrito"))
            {
                var dato = await _context.Distrito.Where(x => x.Estado == true).OrderBy(x=>x.Descripcion).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("PlanAnual"))
            {
                var dato = await _context.PlanAnual.Where(x => x.Estado == true).ToListAsync();

                foreach (var item in dato)
                {
                    MaestroResponseDto m = new MaestroResponseDto();
                    m.Id = item.Id;
                    m.Descripcion = item.AnioPlan;
                    dto.Add(m);
                }
                
            }
            else if (attribute.Equals("PlanQuinquenal"))
            {
                var dato = await _context.PQuinquenal.Where(x => x.Estado == true).ToListAsync();

                foreach (var item in dato)
                {
                    MaestroResponseDto m = new MaestroResponseDto();
                    m.Id = item.Id;
                    m.Descripcion = item.AnioPlan;
                    dto.Add(m);
                }
            }
            else if (attribute.Equals("TipoProyecto"))
            {
                var dato = await _context.TipoProyecto.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("TipoRegistro"))
            {
                var dato = await _context.TipoRegistro.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("Constructor"))
            {
                var dato = await _context.Constructor.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }
            else if (attribute.Equals("EstadoGeneral"))
            {
                var dato = await _context.EstadoGeneral.Where(x => x.Estado == true).ToListAsync();

                var resultadoMap = mapper.Map<List<MaestroResponseDto>>(dato);
                dto = resultadoMap;
            }


            return dto;
           
        }
        public async Task<PaginacionResponseDto<MaestroResponseDto>> GetAll(ListEntidadDTO entidad)
        {

            PaginacionResponseDto<MaestroResponseDto> obj = new PaginacionResponseDto<MaestroResponseDto>();

            if (entidad.Entidad.ToUpper().Equals("Material".ToUpper()))
            {
                var queryable = _context.Material
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
               
            }
            if (entidad.Entidad.Equals("TipoUsuario"))
            {
                var queryable = _context.TipoUsuario
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;

            }
            if (entidad.Entidad.Equals("EstadoPQ"))
            {
                var queryable = _context.EstadoAprobacion
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;

            }
            if (entidad.Entidad.Equals("ProblematicaReal"))
            {
                var queryable = _context.ProblematicaReal
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;

            }
            if (entidad.Entidad.Equals("TipoImpedimento"))
            {
                var queryable = _context.TipoImpedimento
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;

            }
            if (entidad.Entidad.Equals("Distrito"))
            {
                var queryable = _context.Distrito
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true)
                                 .OrderByDescending(x=>x.Descripcion)
                                 .AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
            }
          
            if (entidad.Entidad.Equals("TipoProyecto"))
            {
                var queryable = _context.TipoProyecto
                                 .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                 .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
            }
            if (entidad.Entidad.Equals("TipoRegistro"))
            {
                var queryable = _context.TipoRegistro
                                .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
            }
            if (entidad.Entidad.Equals("Constructor"))
            {
                var queryable = _context.Constructor
                                .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
            }
            if (entidad.Entidad.Equals("EstadoGeneral"))
            {
                var queryable = _context.EstadoGeneral
                                .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                .Where(x => entidad.Estado != null ? x.Estado == entidad.Estado : true).AsQueryable();

                int cantidad = await queryable.CountAsync();
                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();

                var mapEntidad = mapper.Map<List<MaestroResponseDto>>(entidades);
                obj.Cantidad = cantidad;
                obj.Model = mapEntidad;
            }

            return obj;
           
           
            

        }
        public async Task<MaestroResponseDto> GetById(int id, string maestro)
        {
            MaestroResponseDto dto = new MaestroResponseDto();

            if (maestro.ToUpper().Equals("Material".ToUpper()))
            {
                var dato = await _context.Material.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
                
            }
            if (maestro.Equals("TipoUsuario"))
            {
                var dato = await _context.TipoUsuario.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;

            }
            if (maestro.Equals("TipoImpedimento"))
            {
                var dato = await _context.TipoImpedimento.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;

            }
            if (maestro.Equals("ProblematicaReal"))
            {
                var dato = await _context.ProblematicaReal.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;

            }
            if (maestro.Equals("EstadoPQ"))
            {
                var dato = await _context.EstadoAprobacion.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;

            }
            if (maestro.Equals("Distrito"))
            {
                var dato = await _context.Distrito.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("PlanAnual"))
            {
                var dato = await _context.PlanAnual.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("PlanQuinquenal"))
            {
                var dato = await _context.PQuinquenal.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("TipoProyecto"))
            {
                var dato = await _context.TipoProyecto.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("TipoRegistro"))
            {
                var dato = await _context.TipoRegistro.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("Constructor"))
            {
                var dato = await _context.Constructor.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }
            if (maestro.Equals("EstadoGeneral"))
            {
                var dato = await _context.EstadoGeneral.Where(x => x.Id == id).FirstOrDefaultAsync();

                dto.Id = dato.Id;
                dto.Descripcion = dato.Descripcion;
            }


            return dto;
        }

        public async Task<bool> DeleteById(int id,string maestro)
        {

            //var existe = await _context.TablaLogicaDatos.AnyAsync(x => x.IdTablaLogicaDatos == id);
            try
            {


                if (maestro.ToUpper().Equals("Material".ToUpper()))
                {
                    var resultado = await _context.Material.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;

                }
                if (maestro.Equals("TipoUsuario"))
                {
                    var resultado = await _context.TipoUsuario.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;

                }
                if (maestro.Equals("EstadoPQ"))
                {
                    var resultado = await _context.EstadoAprobacion.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;

                }
                if (maestro.Equals("ProblematicaReal"))
                {
                    var resultado = await _context.ProblematicaReal.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;

                }
                if (maestro.Equals("TipoImpedimento"))
                {
                    var resultado = await _context.TipoImpedimento.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;

                }
                if (maestro.Equals("Distrito"))
                {
                    var resultado = await _context.Distrito.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("PlanAnual"))
                {
                    var resultado = await _context.PlanAnual.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("PlanQuinquenal"))
                {
                    var resultado = await _context.PQuinquenal.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("TipoProyecto"))
                {
                    var resultado = await _context.TipoProyecto.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("TipoRegistro"))
                {
                    var resultado = await _context.TipoRegistro.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("Constructor"))
                {
                    var resultado = await _context.Constructor.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (maestro.Equals("EstadoGeneral"))
                {
                    var resultado = await _context.EstadoGeneral.Where(x => x.Id == id).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false : true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return true;

            }
            catch (Exception)
            {

                return false;
            }
           

           

        }
        public async Task<bool> Post(PostEntityReqDTO postEntityReqDTO)
        {

            try
            {
                if (postEntityReqDTO.Entidad.ToUpper().Equals("Material".ToUpper()))
                {
                    Material data = new Material();

                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoUsuario"))
                {
                    TipoUsuario data = new TipoUsuario();

                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoImpedimento"))
                {
                    TipoImpedimento data = new TipoImpedimento();

                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("EstadoPQ"))
                {
                    EstadoAprobacion data = new EstadoAprobacion();

                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("ProblematicaReal"))
                {
                    ProblematicaReal data = new ProblematicaReal();

                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("Distrito"))
                {
                    Distrito data = new Distrito();

              
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("PlanAnual"))
                {
                    Material data = new Material();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("PlanQuinquenal"))
                {
                    Material data = new Material();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoProyecto"))
                {
                    TipoProyecto data = new TipoProyecto();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoRegistro"))
                {
                    TipoRegistro data = new TipoRegistro();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("Constructor"))
                {
                    Constructor data = new Constructor();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("EstadoGeneral"))
                {
                    EstadoGeneral data = new EstadoGeneral();

                    
                    data.Descripcion = postEntityReqDTO.Descripcion;
                    data.Estado = true;
                    _context.Add(data);

                    await _context.SaveChangesAsync();
                    return true;
                }
                return true;

            }
            catch (Exception)
            {

                return false;
            }
            

            
        }

        public async Task<bool> Update(PostUpdateEntityDTO postEntityReqDTO, int id)
        {
            try
            {
                if (postEntityReqDTO.Entidad.ToUpper().Equals("Material".ToUpper()))
                {
                    var entidad = await _context.Material.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("EstadoPQ"))
                {
                    var entidad = await _context.EstadoAprobacion.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoImpedimento"))
                {
                    var entidad = await _context.TipoImpedimento.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("ProblematicaReal"))
                {
                    var entidad = await _context.ProblematicaReal.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoUsuario"))
                {
                    var entidad = await _context.TipoUsuario.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("Distrito"))
                {
                    var entidad = await _context.Distrito.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoProyecto"))
                {
                    var entidad = await _context.TipoProyecto.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("TipoRegistro"))
                {
                    var entidad = await _context.TipoRegistro.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("Constructor"))
                {
                    var entidad = await _context.Constructor.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }
                if (postEntityReqDTO.Entidad.Equals("EstadoGeneral"))
                {
                    var entidad = await _context.EstadoGeneral.Where(x => x.Id == id).FirstOrDefaultAsync();

                    entidad.Descripcion = postEntityReqDTO.Descripcion;
                    entidad.Estado = true;
                    _context.Update(entidad);
                    await _context.SaveChangesAsync();
                    return true;
                }


                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
           
        }
    }
}
