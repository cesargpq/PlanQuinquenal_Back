using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectoController : ControllerBase
    {
        private readonly IProyectoRepository _proyectoRepository;

        public ProyectoController(IProyectoRepository proyectoRepository)
        {
            this._proyectoRepository = proyectoRepository;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _proyectoRepository.GetById(id);
            return Ok(resultado);
        }
        [HttpPost]
        public async Task<IActionResult> GetAll(FiltersProyectos filterProyectos)
        {
            var resultado = await _proyectoRepository.GetAll(filterProyectos);
            return Ok(resultado);
        }
    }
}
