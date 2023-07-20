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
    public class ImpedimentoController : ControllerBase
    {
        private readonly IImpedimentoRepository _impedimentoRepository;

        public ImpedimentoController(IImpedimentoRepository impedimentoRepository)
        {
            this._impedimentoRepository = impedimentoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add(ImpedimentoRequestDTO p)
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
            var resultado = await _impedimentoRepository.Add(p, idUser);

            return Ok(resultado);
        }
        [HttpPut("id")]
        public async Task<IActionResult> Update(ImpedimentoUpdateDto p, int id)
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
            var resultado = await _impedimentoRepository.Update(p, idUser, id);

            return Ok(resultado);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _impedimentoRepository.GetById(id);
            return Ok(resultado);
        }

        [HttpPost("Documentos")]
        public async Task<IActionResult> Documentos(ImpedimentoDocumentoDto p)
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
            var resultado = await _impedimentoRepository.Documentos(p, idUser);

            return Ok(resultado);
        }

        [HttpPost("ListarDocumentos")]
        public async Task<IActionResult> ListarDocumentos(ListaDocImpedimentosDTO p)
        {

            var resultado = await _impedimentoRepository.ListarDocumentos(p);

            return Ok(resultado);
        }

        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(ImpedimentoListDto p)
        {

            var resultado = await _impedimentoRepository.Listar(p);

            return Ok(resultado);
        }

        [HttpGet("Descargar")]
        public async Task<IActionResult> Descargar(int id)
        {
            var resultado = await _impedimentoRepository.Download(id);
            string nombrearchivo = Path.GetFileName(resultado.nombreArchivo).Trim();
            byte[] fl = System.IO.File.ReadAllBytes(resultado.ruta);
            return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, nombrearchivo);
        }
        [HttpPost("ImpedimentosImport")]
        public async Task<IActionResult> ProyectoImport(RequestMasivo data)
        {
            var resultado = await _impedimentoRepository.ProyectoImport(data);
            return Ok(resultado);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
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

            var resultado = await _impedimentoRepository.Delete(id, idUser);
            return Ok(resultado);
        }
        [HttpDelete("DeleteDocumentos")]
        public async Task<IActionResult> DeleteDocumentos(int id)
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

            var resultado = await _impedimentoRepository.DeleteDocumentos(id, idUser);
            return Ok(resultado);
        }

    }
}
