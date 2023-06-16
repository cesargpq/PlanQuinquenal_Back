using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadNegocioController : ControllerBase
    {
        private readonly IRepositoryUnidadNeg _repositoryUnidadNeg;

        public UnidadNegocioController(IRepositoryUnidadNeg repositoryUnidadNeg)
        {
            _repositoryUnidadNeg = repositoryUnidadNeg;
        }

        [HttpPost("NuevoUnidadNeg")]
        public async Task<IActionResult> NuevoUnidadNeg(Unidad_negocio nvoUnidNeeg)
        {

            var resultado = await _repositoryUnidadNeg.NuevoUnidadNeg(nvoUnidNeeg);
            return Ok(resultado);
        }

        [HttpPost("ObtenerUnidadNeg")]
        public async Task<IActionResult> ObtenerUnidadNeg(UnidadNegocioDto entidad)
        {

            var resultado = await _repositoryUnidadNeg.ObtenerUnidadNeg(entidad);
            return Ok(resultado);
        }

        [HttpDelete("EliminarUnidadNeg")]
        public async Task<IActionResult> EliminarUnidadNeg(int cod_uniNeg)
        {

            var resultado = await _repositoryUnidadNeg.EliminarUnidadNeg(cod_uniNeg);
            return Ok(resultado);
        }

        [HttpPut("ActualizarUnidadNeg")]
        public async Task<IActionResult> ActualizarUnidadNeg(Unidad_negocio uniNeg)
        {

            var resultado = await _repositoryUnidadNeg.ActualizarUnidadNeg(uniNeg);

            return Ok(resultado);
        }
    }
}
