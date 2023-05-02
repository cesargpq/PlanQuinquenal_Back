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
        public async Task<LoginResponseDTO> Post(LoginRequestDTO loginRequestDTO)
        {
            LoginResponseDTO lstRsp = new LoginResponseDTO();
            string correoParam = loginRequestDTO.user;

            var queryable = _context.Usuario
                                     .Where(x => x.correo_usu == loginRequestDTO.user)
                                     .Where(x => x.passw_user == loginRequestDTO.password)
                                     .AsQueryable();

            double cantidad = await queryable.CountAsync();
            if (cantidad == 1)
            {
                lstRsp.idMensaje = "1";
                lstRsp.mensaje = "Autencicacion correcta";
                var lstBDPermModulo = await _context.Perm_viz_modulo.FromSqlInterpolated($"EXEC sp_getPermissionsByUser {correoParam} , 'perm_viz_modulo'").ToListAsync();
                var lstBDPermCampos =  _context.Permisos_viz_seccion.FromSqlInterpolated($"EXEC sp_getPermissionsByUser {correoParam} , 'Permisos_viz_seccion'").ToList();
                lstRsp.perm_modulos = lstBDPermModulo;
                lstRsp.perm_campos = lstBDPermCampos;
            }
            else
            {
                lstRsp.idMensaje = "0";
                lstRsp.mensaje = "Datos de logueo invalidos";
            }

            return lstRsp;


        }
    }
}
