using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly PlanQuinquenalContext _context;
        private readonly Constantes _constante;

        public UsuarioController(IUsuarioRepository usuarioRepository, PlanQuinquenalContext context,Constantes constante)
        {
            this.usuarioRepository = usuarioRepository;
            this._context = context;
            this._constante = constante;
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
