using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Repositories;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BolsaReemplazoController : ControllerBase
    {
        private readonly IBolsaReemplazoRepository _bolsaReemplazoRepository;

        public BolsaReemplazoController(IBolsaReemplazoRepository bolsaReemplazoRepository)
        {
            this._bolsaReemplazoRepository = bolsaReemplazoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add(RequestBolsaDto requestBolsaDto)
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

            var resultado = await _bolsaReemplazoRepository.Add(requestBolsaDto,idUser);
            return Ok(resultado);
        }

        [HttpPut]
        public async Task<IActionResult> Update(RequestUpdateBolsaDTO p, int id)
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

            var resultado = await _bolsaReemplazoRepository.Update(p, id, idUser);
            return Ok(resultado);
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(BolsaRequestList requestBolsaDto)
        {
            
            var resultado = await _bolsaReemplazoRepository.Listar(requestBolsaDto);
            return Ok(resultado);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _bolsaReemplazoRepository.GetById(id);
            return Ok(resultado);
        }
        [HttpPost("ImportarMasivo")]
        public async Task<IActionResult> ImportarMasivo(RequestMasivo requestBolsaDto)
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
            var resultado = await _bolsaReemplazoRepository.ImportarMasivo(requestBolsaDto, idUser);
            return Ok(resultado);
        }

        [HttpPost("GestionReemplazo")]
        public async Task<IActionResult> GestionReemplazo(GestionReemplazoDto p)
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
            var resultado = await _bolsaReemplazoRepository.GestionReemplazo(p, idUser);
            return Ok(resultado);
        }
    }
}
