using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using System.IO;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentosRepository _documentosRepository;

        public DocumentosController(IDocumentosRepository documentosRepository) 
        {
            this._documentosRepository = documentosRepository;
        }

        [HttpGet()]
        public async Task<IActionResult> GetDocument(int id,string modulo)
        {
            var resultado = await _documentosRepository.GetUrl(id, modulo);
            return Ok(resultado);
        }

        [HttpGet("Descargar")]
        public async Task<IActionResult> Descargar(int id, string modulo)
        {
            var resultado = await _documentosRepository.Download(id, modulo);
            if(resultado!= null)
            {
                var net = new System.Net.WebClient();
                var data = net.DownloadData(resultado.ruta);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = resultado.nombreArchivo;
                return File(content, contentType, fileName);
            }
            else
            {
                return Ok(new ResponseDTO
                {
                    Valid = true,
                    Message =Constantes.ErrorSistema
                });
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Add(DocumentoRequestDto documentoRequestDto)
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
            var resultado = await _documentosRepository.Add(documentoRequestDto, idUser);
            return Ok(resultado);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id, string modulo)
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

            var resultado = await _documentosRepository.Delete(id, modulo, idUser);
            return Ok(resultado);
        }
    }
}
