using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformesActasController : ControllerBase
    {
        private readonly IInforemesActasRepository _inforemesActasRepository;

        public InformesActasController(IInforemesActasRepository inforemesActasRepository)
        {
            this._inforemesActasRepository = inforemesActasRepository;
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear(InformeReqDTO informeReqDTO)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var idUser = 0;

            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUser = Convert.ToInt16(item.Value);
                }
            }
            var resultado = await _inforemesActasRepository.Crear(informeReqDTO,idUser);
            return Ok(resultado);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _inforemesActasRepository.GetById(id);
            return Ok(resultado);
        }
    }
}
