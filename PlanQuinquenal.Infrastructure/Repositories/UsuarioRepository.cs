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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PlanQuinquenalContext _context;

        public UsuarioRepository(PlanQuinquenalContext context) 
        {
            this._context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAll(UsuarioListDTO entidad)
        {
            var usuarios = await _context.Usuario.ToListAsync();
            var queryable = _context.Usuario
                                     .AsQueryable();

           
            var entidades = await queryable.OrderBy(e => e.nombre_usu).Paginar(entidad)
                                   .ToListAsync();
            double cantidad =  entidades.Count();
            return usuarios;
        }

        public async Task<Usuario> GetById(int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync( x => x.cod_usu == id);
            
            return resultado;
        }

        public async Task<bool> Update(Usuario usuario, int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync(x => x.cod_usu == id);

            if (resultado != null)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateState(int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync(x => x.cod_usu == id);

            if (resultado != null)
            {
                resultado.estado_user = resultado.estado_user == "A" ? "D" : "A";
                _context.Add(resultado);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CreateUser(Usuario usuario)
        {
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
