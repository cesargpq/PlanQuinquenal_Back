using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Repositories;
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

        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(PaginationFilterActaDto id)
        {
            var resultado = await _inforemesActasRepository.GetAll(id);
            return Ok(resultado);
        }
        [HttpPut("id")]
        public async Task<IActionResult> Update(InformeReqDTO informeReqDTO,int id)
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
            var resultado = await _inforemesActasRepository.Update(informeReqDTO, id, idUser);
            return Ok(resultado);
        }

        [HttpGet("Descargar")]
        public async Task<IActionResult> Descargar(int id)
        {
            var resultado = await _inforemesActasRepository.Download(id);
            var guid =  Guid.NewGuid();
            
            byte[] fl = System.IO.File.ReadAllBytes(resultado.ruta);
            return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, guid+".pdf");


        }

        [HttpPut("AprobarActa")]
        public async Task<IActionResult> AprobarActa(AprobarActaDto a)
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
            var resultado = await _inforemesActasRepository.AprobarActa(a,idUser);

            return Ok(resultado);
        }
    }
}
