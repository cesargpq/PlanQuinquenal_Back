using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly IRepositoryPerfil _repositoryPerfil;

        public PerfilController(IRepositoryPerfil repositoryPerfil)
        {
            _repositoryPerfil = repositoryPerfil;
        }

        [HttpPost("NuevoPerfil")]
        public async Task<IActionResult> NuevoPerfil(PerfilResponse nvoPerfil)
        {
            var resultado = await _repositoryPerfil.NuevoPerfil(nvoPerfil);
            return Ok(resultado);
        }

        [HttpPost("ObtenerPerfiles")]
        public async Task<IActionResult> ObtenerPerfiles(PerfilListDto buscador)
        {

            var resultado = await _repositoryPerfil.ObtenerPerfiles(buscador);
            return Ok(resultado);
        }

        [HttpDelete("EliminarPerfil")]
        public async Task<IActionResult> EliminarPerfil(int cod_perfil)
        {

            var resultado = await _repositoryPerfil.EliminarPerfil(cod_perfil);
            return Ok(resultado);
        }

        [HttpPut("ActualizarPerfil")]
        public async Task<IActionResult> ActualizarPerfil(PerfilResponse nvoPerfil)
        {
            var resultado = await _repositoryPerfil.ActualizarPerfil(nvoPerfil);
            return Ok(resultado);
        }
    }
}
