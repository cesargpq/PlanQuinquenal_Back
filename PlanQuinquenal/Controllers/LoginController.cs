using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRepositoryLogin _repositoryLogin;

        public LoginController(IRepositoryLogin repositoryLogin)
        {
            _repositoryLogin = repositoryLogin;
        }
        [AllowAnonymous]
        [HttpPost("AutenticarUsuario")]
        public async Task<IActionResult> AutenticarUsuario(LoginRequestDTO reqLogin)
        {
            var resultado = await _repositoryLogin.Post(reqLogin);
            return Ok(resultado);
        }

        [HttpGet("ObtenerModulos")]
        public async Task<IActionResult> GetById()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims.ElementAt(1) != null)
            {
                var correo = identity.Claims.ElementAt(1).Value;
                var resultado = await _repositoryLogin.ObtenerModulos(correo);
                return Ok(resultado);
            }
            else
            {
                ModulosResponse resp =  new ModulosResponse();
                resp.idMensaje = "0";
                resp.mensaje = "Hubo un error al momento de obtener su informacion";
                return Ok(resp);
            }

            
        }

        [HttpGet("ObtenerSeccionesMod")]
        public async Task<IActionResult> ObtenerSecciones(string modulo, string seccion)
        {

            var resultado = await _repositoryLogin.ObtenerSecciones(modulo, seccion);
            return Ok(resultado);
        }
    }
}
