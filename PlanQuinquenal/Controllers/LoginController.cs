using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRepositoryLogin _repositoryLogin;

        public LoginController(IRepositoryLogin repositoryLogin)
        {
            _repositoryLogin = repositoryLogin;
        }

        [HttpPost("AutenticarUsuario")]
        public async Task<IActionResult> AutenticarUsuario(LoginRequestDTO reqLogin)
        {
            var resultado = await _repositoryLogin.Post(reqLogin);
            return Ok(resultado);
        }

        [HttpGet("ObtenerModulos")]
        public async Task<IActionResult> GetById(string correo)
        {

            var resultado = await _repositoryLogin.ObtenerModulos(correo);
            return Ok(resultado);
        }

        [HttpGet("ObtenerSeccionesMod")]
        public async Task<IActionResult> ObtenerSecciones(string modulo, string seccion)
        {

            var resultado = await _repositoryLogin.ObtenerSecciones(modulo, seccion);
            return Ok(resultado);
        }
    }
}
