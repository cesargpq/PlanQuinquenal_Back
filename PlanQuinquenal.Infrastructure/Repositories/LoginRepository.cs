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
                //var query = _context.Usuario.Join(_context.Perfil, usuario => usuario.cod_perfil, perfil => perfil.cod_perfil,
                //    (usuario, perfil) => new { Usuario = usuario, Perfil = perfil }).Where(x => x.Usuario.correo_usu == loginRequestDTO.user).ToList();
                var queryable2 = await _context.Usuario.Include(x => x.Perfil).ThenInclude(y => y.Perm_viz_modulo).Include(z => 
                z.Perfil.Permisos_viz_seccion = _context.Permisos_viz_seccion.Where(x => x.codSec_permViz == z.Perfil.Permisos_viz_seccioncodSec_permViz))
                    .ToListAsync();
                //var entidades = await queryable2.ToListAsync();
                lstRsp.idMensaje = "1";
                lstRsp.mensaje = "Autencicacion correcta";
                //lstRsp.nombre= entidades[0].nombre_usu;
                //lstRsp.apellido = entidades[0].apellido_usu;
                //lstRsp.correo = entidades[0].correo_usu;
                //var lstBDPermModulo = await _context.Perm_viz_modulo.FromSqlInterpolated($"EXEC sp_getPermissionsByUser {correoParam} , 'perm_viz_modulo'").ToListAsync();
                //var lstBDPermCampos =  _context.Permisos_viz_seccion.FromSqlInterpolated($"EXEC sp_getPermissionsByUser {correoParam} , 'Permisos_viz_seccion'").ToList();
                //lstRsp.perm_modulos = lstBDPermModulo;
                //lstRsp.perm_campos = lstBDPermCampos;
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
