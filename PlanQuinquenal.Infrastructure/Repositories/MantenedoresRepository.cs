using ApiDavis.Core.Utilidades;
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

        public MantenedoresRepository(PlanQuinquenalContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<TablaLogicaDatos>> GetAllByAttribute(string attribute)
        {
            var dato = await _context.TablaLogica.Where(x => x.Descripcion == attribute).ToListAsync();
            var datoFinal = await _context.TablaLogicaDatos.Where(x => x.TablaLogicaId == dato.ElementAt(0).Id).ToListAsync();
            return datoFinal;
        }
        public async Task<PaginacionResponseDto<TablaLogicaDatos>> GetAll(ListEntidadDTO entidad)
        {

            
            var dato  = await _context.TablaLogica.Where(x => x.Descripcion== entidad.Entidad).ToListAsync();
          

            var queryable = _context.TablaLogicaDatos
                                     .Where(x => x.TablaLogicaId == dato.ElementAt(0).Id)
                                     .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                     .Where(x => entidad.Valor != "" ? x.Descripcion == entidad.Valor : true)
                                     .Where(x => entidad.Codigo != "" ? x.Descripcion == entidad.Codigo : true)
                                     .AsQueryable();

            int cantidad = await queryable.CountAsync();
            var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                   .ToListAsync();

            var objeto = new PaginacionResponseDto<TablaLogicaDatos>
            {
                Cantidad = cantidad,
                Model = entidades
            };

            return objeto;

        }
        public async Task<TablaLogicaDatos> GetById(int id)
        {

            var dato = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id).FirstOrDefaultAsync();
            
        
            return dato;

        }

        public async Task<bool> DeleteById(int id)
        {

            var existe = await _context.TablaLogicaDatos.AnyAsync(x => x.IdTablaLogicaDatos == id);
            try
            {
                if (!existe)
                {

                    return false;
                }
                else
                {
                    var resultado = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id ).FirstOrDefaultAsync();
                    resultado.Estado = resultado.Estado == true ? false: true;
                    _context.Update(resultado);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
           

           

        }
        public async Task<bool> Post(PostEntityReqDTO postEntityReqDTO)
        {
            var dato = await _context.TablaLogica.Where(x => x.Descripcion == postEntityReqDTO.Entidad).ToListAsync();

            try
            {
                TablaLogicaDatos data = new TablaLogicaDatos();

                data.TablaLogicaId = dato.ElementAt(0).Id;
                data.Descripcion = postEntityReqDTO.Descripcion;
                data.Codigo = postEntityReqDTO.Codigo;
                data.Valor = postEntityReqDTO.Valor;
                data.Estado = true;
                _context.Add(data);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            

            
        }

        public async Task<bool> Update(PostUpdateEntityDTO postEntityReqDTO, int id)
        {
            var dato = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id).ToListAsync();

            if (dato == null)
            {

                return false;
            }
            else
            {
                TablaLogicaDatos data = new TablaLogicaDatos();
                var resultado = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id ).FirstOrDefaultAsync();
                if(resultado != null)
                {
              
                    resultado.Descripcion = postEntityReqDTO.Descripcion;
                    resultado.Estado = postEntityReqDTO.Estado;
                    resultado.Codigo = postEntityReqDTO.Codigo;
                    resultado.Valor = postEntityReqDTO.Valor;
                    _context.Update(resultado);

                    await _context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
           
        }
    }
}
