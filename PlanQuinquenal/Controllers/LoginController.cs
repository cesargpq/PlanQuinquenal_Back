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
    }
}
