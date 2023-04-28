using Microsoft.Data.SqlClient;
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
    public class LoginRepository : IRepositoryLogin
    {
        private readonly PlanQuinquenalContext _context;

        public LoginRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }
        public async Task<bool> Post(LoginRequestDTO loginRequestDTO)
        {
            var correoParam = new SqlParameter("@correo", loginRequestDTO.user);

            var prueba = await _context.Permisos_viz_seccion.FromSql("EXEC sp_getPermissionsByUser @correo", correoParam).ToListAsync();
            return true;
        }
        public async Task<bool> Post(PostEntityReqDTO postEntityReqDTO)
        {
            var dato = await _context.TablaLogica.Where(x => x.Descripcion == postEntityReqDTO.Entidad).ToListAsync();

            TablaLogicaDatos data = new TablaLogicaDatos();

            data.IdTablaLogica = dato.ElementAt(0).IdTablaLogica;
            data.Descripcion = postEntityReqDTO.Descripcion;
            data.Codigo = postEntityReqDTO.Codigo;
            data.Valor = postEntityReqDTO.Valor;
            _context.Add(data);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
