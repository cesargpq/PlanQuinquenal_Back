using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(UsuarioListDTO usuarioListDTO)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> GetAll(UsuarioListDTO usuarioListDTO)
        {
            var resultado = await usuarioRepository.GetAll(usuarioListDTO);
            return Ok();
        }
    }
}
