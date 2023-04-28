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

        [HttpPost("ListarEntidad")]
        public async Task<IActionResult> GetAll(ListEntidadDTO entidad)
        {

            var resultado = await _repositoryMantenedores.GetAll(entidad);
            return Ok(resultado);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {

            var resultado = await _repositoryMantenedores.GetById(id);
            return Ok(resultado);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteById(int id)
        {

            var resultado = await _repositoryMantenedores.DeleteById(id);
            return Ok(resultado);
        }
        [HttpPost]
        public async Task<IActionResult> Post(PostEntityReqDTO postEntityReqDTO)
        {

            var resultado = await _repositoryMantenedores.Post(postEntityReqDTO);

            return Ok(resultado);
        }
        [HttpPut("id")]
        public async Task<IActionResult> Update(PostUpdateEntityDTO postEntityReqDTO,int id)
        {

            var resultado = await _repositoryMantenedores.Update(postEntityReqDTO,id);

            return Ok(resultado);
        }
    }
}
