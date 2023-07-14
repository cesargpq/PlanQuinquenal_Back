using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IRepositoryMantenedores r;

        public ComentariosController(IComentarioRepository comentarioRepository , IRepositoryMantenedores r)
        {
            this._comentarioRepository = comentarioRepository;
            this.r = r;
        }

        [HttpPost]
        public async Task<IActionResult> Add(ComentarioRequestDTO req)
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
            var resultado = await _comentarioRepository.Add(req,idUser);
            return Ok(resultado);
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(RequestComentarioDTO p)
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
            var tipo = await r.GetAllByAttribute("TipoSeguimiento");

            var tipoSe = tipo.Where(x => x.Id == p.TipoSeguimientoId).FirstOrDefault();
            if (tipoSe.Descripcion.Equals("Proyectos"))
            {
                var resultado = await _comentarioRepository.ListarPY(p, idUser);
                return Ok(resultado);
            }
            else if (tipoSe.Descripcion.Equals("Plan Quinquenal"))
            {
                var resultado = await _comentarioRepository.ListarPQ(p, idUser);
                return Ok(resultado);
            }
            else
            {
                var resultado = await _comentarioRepository.ListarPA(p, idUser);
                return Ok(resultado);
            }



        }
    }
}
