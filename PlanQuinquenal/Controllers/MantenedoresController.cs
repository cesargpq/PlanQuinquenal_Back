using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MantenedoresController : ControllerBase
    {
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public MantenedoresController(IRepositoryMantenedores repositoryMantenedores )
        {
            _repositoryMantenedores = repositoryMantenedores;
        }

        [HttpGet("entidad")]
        public async Task<IActionResult> GetAll(string entidad)
        {

            var resultado = await _repositoryMantenedores.GetAll(entidad);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostEntityReqDTO postEntityReqDTO)
        {

            var resultado = await _repositoryMantenedores.Post(postEntityReqDTO);

            return Ok(resultado);
        }
    }
}
