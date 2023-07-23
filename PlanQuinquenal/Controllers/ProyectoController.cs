using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Net;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> Add(ProyectoRequestDto proyectoRequestDto)
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
            var resultado = await _proyectoRepository.Add(proyectoRequestDto, usuario);

            return Ok(resultado);
        }

        [HttpPost("ProyectoImport")]
        public async Task<IActionResult> ProyectoImport(RequestMasivo data)
        {
            var resultado = await _proyectoRepository.ProyectoImport(data);
            return Ok(resultado);
        }
        //[HttpPost("Listar")]
        //public async Task<IActionResult> GetAll(FiltersProyectos filterProyectos)
        //{
        //    var resultado = await _proyectoRepository.GetAll(filterProyectos);
        //    return Ok(resultado);
        //}
        [HttpPost("Listar")]
        public async Task<IActionResult> GetAll2(FiltersProyectos filterProyectos)
        {
            var resultado = await _proyectoRepository.GetAll2(filterProyectos);
            return Ok(resultado);
        }
        [HttpPut("id")]
        public async Task<IActionResult> Update(ProyectoRequestUpdateDto proyecto, int id)
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
            var resultado = await _proyectoRepository.Update(proyecto,id,idUser);
            return Ok(resultado);
        }
        [HttpPost("ListarEtapas")]
        public async Task<IActionResult> ListarEtapas(EtapasListDto filterProyectos)
        {
            var resultado = await _proyectoRepository.ListarEtapas(filterProyectos);
            return Ok(resultado);
        }
    }
}
