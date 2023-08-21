using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Repositories;
using System.Net;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosProyectoController : ControllerBase
    {
        private readonly IPermisosProyectoRepository _permisosProyectoRepository;

        public PermisosProyectoController(IPermisosProyectoRepository permisosProyectoRepository)
        {
            this._permisosProyectoRepository = permisosProyectoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add(PermisoRequestDTO permisoRequestDTO)
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

            var resultado = await _permisosProyectoRepository.Add(permisoRequestDTO,  usuario);
            return Ok(resultado);
        }
        [HttpGet]
        public async Task<IActionResult> GetPermiso(string CodigoProyecto, string TipoPermiso)
        {
            var resultado = await _permisosProyectoRepository.GetPermiso(CodigoProyecto, TipoPermiso);

            return Ok(resultado);
        }

        [HttpPost("CargarExpediente")]
        public async Task<IActionResult> CargarExpediente(DocumentosPermisosRequestDTO documentosPermisosRequestDTO)
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
            var resultado = await _permisosProyectoRepository.CargarExpediente(documentosPermisosRequestDTO, usuario);
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
            DatosUsuario usuario = new DatosUsuario();
            usuario.Ip = (HttpContext.Items["PublicIP"] as IPAddress).ToString(); ;
            usuario.UsuaroId = idUser;

            var resultado = await _permisosProyectoRepository.Delete(id, usuario);
            return Ok(resultado);
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(ListDocumentosRequestDto listDocumentosRequestDto)
        {

            var resultado = await _permisosProyectoRepository.Listar(listDocumentosRequestDto);
            return Ok(resultado);
        }

        [HttpGet("Descargar")]
        public async Task<IActionResult> Descargar(int id)
        {
            var resultado = await _permisosProyectoRepository.Download(id);
          
            string codigoArchivo = Path.GetFileName(resultado.Model.CodigoDocumento).Trim();
            byte[] fl = System.IO.File.ReadAllBytes(resultado.Model.ruta);
            return File(fl, System.Net.Mime.MediaTypeNames.Application.Octet, codigoArchivo);

        }
    }
}
