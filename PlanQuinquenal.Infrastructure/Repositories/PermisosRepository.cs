using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PermisosRepository : IRepositoryPermisos
    {
        private readonly PlanQuinquenalContext _context;

        public PermisosRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }
        public async Task<ColumsTablaPerResponse> VisColumnTabla(string correo, string tabla)
        {
            ColumsTablaPerResponse respMod = new ColumsTablaPerResponse();

            var secciones = new List<int>();
            var queryable = await _context.ColumTablaUsu
                                     .Where(x => x.correo == correo)
                                     .Where(x => x.tabla == tabla)
                                     .ToListAsync();

            if (queryable.Count() > 0)
            {

                respMod.idMensaje = "1";
                respMod.mensaje = "Permisos obtenidos correctamente";
                respMod.perm_ColumTabla = queryable;
            }
            else
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "Su usuario no cuenta con Permisos de visualizacion de esta tabla";
            }


            return respMod;
        }
    }
}
