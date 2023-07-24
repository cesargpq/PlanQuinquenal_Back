using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using System.Net;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanQuinquenalController : ControllerBase
    {
        private readonly IPlanQuinquenalesRepository planQuinquenalesRepository;

        public PlanQuinquenalController(IPlanQuinquenalesRepository planQuinquenalesRepository)
        {
            this.planQuinquenalesRepository = planQuinquenalesRepository;
        }


        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(PQuinquenalRequestDTO p)
        {
            var resultado = await planQuinquenalesRepository.GetAll(p);
            return Ok(resultado);
        }
        [HttpPost]
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
            DatosUsuario usuario = new DatosUsuario();
            usuario.Ip = (HttpContext.Items["PublicIP"] as IPAddress).ToString(); ;
            usuario.UsuaroId = idUser;
            var resultado = await planQuinquenalesRepository.Add(p,usuario);
            return Ok(resultado);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await planQuinquenalesRepository.GetById(id);
            return Ok(resultado);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePlanQuinquenalDto dto,int id)
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
            DatosUsuario usuario = new DatosUsuario();
            usuario.Ip = (HttpContext.Items["PublicIP"] as IPAddress).ToString(); ;
            usuario.UsuaroId = idUser;


            var resultado = await planQuinquenalesRepository.Update(dto, id, usuario);
            return Ok(resultado);
        }

    }
}
