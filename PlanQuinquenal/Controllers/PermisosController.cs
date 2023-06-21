using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly IRepositoryPermisos _repositoryPermisos;

        public PermisosController(IRepositoryPermisos repositoryPermisos)
        {
            _repositoryPermisos = repositoryPermisos;
        }

        [HttpGet("VisColumnTabla")]
        public async Task<IActionResult> VisColumnTabla(string tabla)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims.ElementAt(1) != null)
            {
                var identityCl = HttpContext.User.Identity as ClaimsIdentity;
                var idUser = 0;
                foreach (var item in identityCl.Claims)
                {
                    if (item.Type.Equals("$I$Us$@I@D"))
                    {
                        idUser = Convert.ToInt16(item.Value);

                    }
                }
                var resultado = await _repositoryPermisos.VisColumnTabla(idUser, tabla);
                return Ok(resultado);
            }
            else
            {
                ColumsTablaPerResponse resp = new ColumsTablaPerResponse();
                resp.idMensaje = "0";
                resp.mensaje = "Hubo un error al momento de obtener su informacion";
                return Ok(resp);
            }

            
        }

        [HttpPost("ActualizarPermisosMod")]
        public async Task<IActionResult> ActualizarPermisosMod(TablaPerm_viz_modulo reqModulos)
        {
            var resultado = await _repositoryPermisos.ActualizarPermisosMod(reqModulos);
            return Ok(resultado);
        }

        [HttpPost("ActAccionesPerfil")]
        public async Task<IActionResult> ActAccionesPerfil(List<Acciones_Rol> reqAccRol)
        {
            var resultado = await _repositoryPermisos.ActAccionesPerfil(reqAccRol);
            return Ok(resultado);
        }

        [HttpGet("ConsAccionesPerfil")]
        public async Task<IActionResult> ConsAccionesPerfil()
        {
            var resultado = await _repositoryPermisos.ConsAccionesPerfil();
            return Ok(resultado);
        }

        [HttpGet("ObtenerConfRolesPerm")]
        public async Task<IActionResult> ObtenerConfRolesPerm()
        {
            var resultado = await _repositoryPermisos.ObtenerConfRolesPerm();
            return Ok(resultado);
        }

        [HttpPost("ActConfRolesPerm")]
        public async Task<IActionResult> ActConfRolesPerm(List<ConfRolesPerm> conRolesPerm)
        {
            var resultado = await _repositoryPermisos.ActConfRolesPerm(conRolesPerm);
            return Ok(resultado);
        }

        [HttpPost("ModPermisoColumnTabla")]
        public async Task<IActionResult> ModPermisoColumnTabla(ColumTablaUsu columna)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var idUser = 0;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUser = Convert.ToInt16(item.Value);

                }
            }
            var resultado = await _repositoryPermisos.ModPermisoColumnTabla(columna, idUser);
            return Ok(resultado);
        }
    }
}
