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

        public Task<LoginResponseDTO> ObtenerModulos(string correo)
        {
            throw new NotImplementedException();
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
                var queryable2 = await _context.Usuario.Include(x => x.Perfil).ThenInclude(y => y.Perm_viz_modulo)
                    .ToListAsync();
                //var entidades = await queryable2.ToListAsync();
                lstRsp.idMensaje = "1";
                lstRsp.mensaje = "Autencicacion correcta";
                lstRsp.nombre = queryable2[0].nombre_usu;
                lstRsp.apellido = queryable2[0].apellido_usu;
                lstRsp.correo = queryable2[0].correo_usu;
                lstRsp.cod_perfil = queryable2[0].Perfil.cod_perfil;
                lstRsp.cod_modulo_perf = queryable2[0].Perfil.Perm_viz_modulocodMod_permiso;
                lstRsp.cod_secs_perf = queryable2[0].Perfil.Permisos_viz_seccioncodSec_permViz;
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

        public async Task<ModulosResponse> ObtenerModulos(string correo)
        {
            ModulosResponse respMod = new ModulosResponse();
            List<TablaPerm_viz_modulo> lstModulos = new List<TablaPerm_viz_modulo>();
            TablaPerm_viz_modulo respModulos = new TablaPerm_viz_modulo();
            var lstBDPermModulo = await _context.Perm_viz_modulo.FromSqlInterpolated($"EXEC sp_getPermissionsByUser {correo} , 'perm_viz_modulo'").ToListAsync();

            var secciones = new List<int>();
            var queryable = await _context.Secc_modulos
                                     .Where(x => x.modulo == "Dashboard")
                                     .ToListAsync();

            if (lstBDPermModulo.Count() > 0)
            {
                foreach (var seccion in queryable)
                {
                    secciones.Add(seccion.id);
                }

                var permSec = _context.Permisos_viz_seccion
                        .Where(p => secciones.Contains(p.cod_seccion))
                        .ToList();

                respMod.idMensaje = "1";
                respMod.mensaje = "Permisos obtenidos correctamente";
                respMod.perm_modulos = lstBDPermModulo[0];
                respMod.perm_campos = permSec;
            }
            else
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "No cuenta con los permisos para ver información";
            }

            
            return respMod;
        }

        public async Task<ModulosResponse> ObtenerSecciones(string modulo, string seccion)
        {
            ModulosResponse respMod = new ModulosResponse();

            var secciones = new List<int>();
            var queryable = await _context.Secc_modulos
                                     .Where(x => x.modulo == modulo)
                                     .ToListAsync();

            if (queryable.Count() > 0)
            {
                foreach (var secc in queryable)
                {
                    secciones.Add(secc.id);
                }

                var permSec = _context.Permisos_viz_seccion
                        .Where(p => secciones.Contains(p.cod_seccion))
                        .ToList();

                respMod.idMensaje = "1";
                respMod.mensaje = "Permisos obtenidos correctamente";
                respMod.perm_campos = permSec;
            }
            else
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "No cuenta con los permisos para ver información";
            }


            return respMod;
        }
    }
}
