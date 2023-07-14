using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Repositories;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanAnualController : ControllerBase
    {
        private readonly IPlanAnualRepository planAnualRepository;

        public PlanAnualController(IPlanAnualRepository planAnualRepository)
        {
            this.planAnualRepository = planAnualRepository;
        }

        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(PQuinquenalRequestDTO p)
        {
            var resultado = await planAnualRepository.GetAll(p);
            return Ok(resultado);
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PQuinquenalReqDTO p)
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
            var resultado = await planAnualRepository.Add(p, idUser);
            return Ok(resultado);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await planAnualRepository.GetById(id);
            return Ok(resultado);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePlanQuinquenalDto dto, int id)
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
            var resultado = await planAnualRepository.Update(dto, id, idUser);
            return Ok(resultado);
        }
    }
}
