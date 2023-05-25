using ApiDavis.Core.Utilidades;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
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
        public async Task<IEnumerable<TablaLogicaDatos>> GetAll(ListEntidadDTO entidad)
        {

            
            var dato  = await _context.TablaLogica.Where(x => x.Descripcion== entidad.Entidad).ToListAsync();
          

            var queryable = _context.TablaLogicaDatos
                                     .Where(x => x.IdTablaLogica == dato.ElementAt(0).Id)
                                     .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                     .Where(x => entidad.Valor != "" ? x.Descripcion == entidad.Valor : true)
                                     .Where(x => entidad.Codigo != "" ? x.Descripcion == entidad.Codigo : true)
                                     .AsQueryable();

            double cantidad = await queryable.CountAsync();
            var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                   .ToListAsync();

            return entidades;

        }
        public async Task<TablaLogicaDatos> GetById(int id)
        {

            //var dato = await _context.TablaLogica.FromSqlInterpolated($"exec Tabla").ToListAsync();
            var prueba = await _context.Perm_viz_modulo.FromSqlInterpolated($"EXEC sp_getPermissionsByUser 'eacosta@xternal.biz' , 'perm_viz_modulo'").ToListAsync();
            var prueba2 = await _context.Permisos_viz_seccion.FromSqlInterpolated($"EXEC sp_getPermissionsByUser 'eacosta@xternal.biz' , 'Permisos_viz_seccion'").ToListAsync();
            var resultado = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id && x.Estado == true).FirstOrDefaultAsync();
        
            return resultado;

        }

        public async Task<bool> DeleteById(int id)
        {

            var existe = await _context.TablaLogicaDatos.AnyAsync(x => x.IdTablaLogicaDatos == id);
            if (!existe)
            {

                return false;
            }
            else
            {
                var resultado = await _context.TablaLogicaDatos.Where(x => x.IdTablaLogicaDatos == id && x.Estado == true).FirstOrDefaultAsync();
                resultado.Estado = false;
                _context.Update(resultado);
                await _context.SaveChangesAsync();
                return true;
            }

           

        }
        public async Task<bool> Post(PostEntityReqDTO postEntityReqDTO)
        {
            var dato = await _context.TablaLogica.Where(x => x.Descripcion == postEntityReqDTO.Entidad).ToListAsync();

            TablaLogicaDatos data = new TablaLogicaDatos();

            data.IdTablaLogica = dato.ElementAt(0).Id;
            data.Descripcion = postEntityReqDTO.Descripcion;
            data.Codigo = postEntityReqDTO.Codigo;
            data.Valor = postEntityReqDTO.Valor;
            _context.Add(data);

            await _context.SaveChangesAsync();

            return true;
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
