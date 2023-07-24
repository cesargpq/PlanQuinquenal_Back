﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using System.IO;
using System.Net;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            string nombrearchivo = Path.GetFileName(resultado.nombreArchivo).Trim();
            byte[] fl = System.IO.File.ReadAllBytes(resultado.ruta);
            return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, nombrearchivo);
          

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
            DatosUsuario usuario = new DatosUsuario();
            usuario.Ip = (HttpContext.Items["PublicIP"] as IPAddress).ToString(); ;
            usuario.UsuaroId = idUser;

            var resultado = await _documentosRepository.Add(documentoRequestDto, usuario);
            return Ok(resultado);
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(ListDocumentosRqDTO listDocumentosRequestDto)
        {
            var resultado = await _documentosRepository.Listar(listDocumentosRequestDto);
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
            DatosUsuario usuario = new DatosUsuario();
            usuario.Ip = (HttpContext.Items["PublicIP"] as IPAddress).ToString(); ;
            usuario.UsuaroId = idUser;

            var resultado = await _documentosRepository.Delete(id, modulo, usuario);
            return Ok(resultado);
        }
    }
}
