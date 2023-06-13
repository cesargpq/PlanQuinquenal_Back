using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public NotificacionesController(IRepositoryNotificaciones repositoryNotificaciones)
        {
            _repositoryNotificaciones = repositoryNotificaciones;
        }

        [HttpGet("ContadorNotifNuevas")]
        public async Task<IActionResult> ContadorNotifNuevas()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    var codUsu = int.Parse(item.Value);
                    var resultado = await _repositoryNotificaciones.ContadorNotifNuevas(codUsu);
                    return Ok(resultado);
                }
            }

            return Ok("Hubo un error en la consulta");
            
        }

        [HttpGet("CambiarEstadoNotif")]
        public async Task<IActionResult> CambiarEstadoNotif(int cod_notif, bool flagVisto)
        {
            var resultado = await _repositoryNotificaciones.CambiarEstadoNotif(cod_notif, flagVisto);
            return Ok(resultado);
        }

        [HttpGet("ObtenerConfigNotif")]
        public async Task<IActionResult> ObtenerConfigNotif()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    var codUsu = int.Parse(item.Value);
                    var resultado = await _repositoryNotificaciones.ObtenerConfigNotif(codUsu);
                    return Ok(resultado);
                }
            }

            return Ok("Hubo un error en la consulta");
            
        }

        [HttpGet("ObtenerListaNotif")]
        public async Task<IActionResult> ObtenerListaNotif()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    var codUsu = int.Parse(item.Value);
                    var resultado = await _repositoryNotificaciones.ObtenerListaNotif(codUsu);
                    return Ok(resultado);
                }
            }

            return Ok("Hubo un error en la consulta");

        }

        [HttpPost("ModificarConfigNotif")]
        public async Task<IActionResult> ModificarConfigNotif(Config_notificaciones config)
        {
            var resultado = await _repositoryNotificaciones.ModificarConfigNotif(config);
            return Ok(resultado);
        }

        [HttpPost("CrearNotificacion")]
        public async Task<IActionResult> CrearNotificacion(Notificaciones notificacion)
        {
            var resultado = await _repositoryNotificaciones.CrearNotificacion(notificacion);
            return Ok(resultado);
        }

        [HttpPost("EnvioCorreoNotif")]
        public async Task<IActionResult> EnvioCorreoNotif(CorreoResponse correo)
        {
            var resultado = "";//await _repositoryNotificaciones.EnvioCorreoNotif(correo);
            return Ok(resultado);
        }

        [HttpPost("CrearConfigNotif")]
        public async Task<IActionResult> CrearConfigNotif(Config_notificaciones config)
        {
            var resultado = await _repositoryNotificaciones.CrearConfigNotif(config);
            return Ok(resultado);
        }
    }
}
