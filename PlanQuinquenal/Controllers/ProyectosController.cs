using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Filters;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly IRepositoryProyecto _repositoryProyecto;

        public ProyectosController(IRepositoryProyecto repositoryProyecto)
        {
            _repositoryProyecto = repositoryProyecto;
        }

        [HttpPost("NuevoProyecto")]
        public async Task<IActionResult> NuevoProyecto(ProyectoRequest nvoProyecto)
        {

            var resultado = await _repositoryProyecto.NuevoProyecto(nvoProyecto);
            return Ok(resultado);
        }

        [HttpPost("ObtenerProyectos")]
        public async Task<IActionResult> ObtenerProyectos(FiltersProyectos filterProyectos)
        {

            var resultado = await _repositoryProyecto.ObtenerProyectos(filterProyectos);
            return Ok(resultado);
        }

        [HttpPost("NuevosProyectosMasivo")]
        public async Task<IActionResult> NuevosProyectosMasivo(ProyectoRequest reqMasivo)
        {

            var resultado = await _repositoryProyecto.NuevosProyectosMasivo(reqMasivo);
            return Ok(resultado);
        }

        [HttpGet("ObtenerProyectoxNro")]
        public async Task<IActionResult> ObtenerProyectoxNro(string nroProy)
        {

            var resultado = await _repositoryProyecto.ObtenerProyectoxNro(nroProy);

            return Ok(resultado);
        }

        [HttpPost("CrearImpedimento")]
        public async Task<IActionResult> CrearImpedimento(ImpedimentoRequest impedimento)
        {

            var resultado = await _repositoryProyecto.CrearImpedimento(impedimento);
            return Ok(resultado);
        }

        [HttpPost("ModificarImpedimento")]
        public async Task<IActionResult> ModificarImpedimento(ImpedimentoRequest impedimento)
        {

            var resultado = "";
            return Ok(resultado);
        }

        [HttpPost("CrearComentario")]
        public async Task<IActionResult> CrearComentario(Comentarios_proyec comentario)
        {

            var resultado = await _repositoryProyecto.CrearComentario(comentario);
            return Ok(resultado);
        }

        [HttpPost("ModificarComentario")]
        public async Task<IActionResult> ModificarComentario(Comentarios_proyec comentario)
        {

            var resultado = "";
            return Ok(resultado);
        }
    }
}
