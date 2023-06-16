using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebaController : ControllerBase
    {
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public PruebaController(IRepositoryNotificaciones repositoryNotificaciones)
        {
            _repositoryNotificaciones = repositoryNotificaciones;
        }

        [HttpPost]
        public async Task<IActionResult> ServicioPrueba(List<CorreoTabla> lstmodif)
        {
            List<Notificaciones> noti = new List<Notificaciones>();
            var resultado = await _repositoryNotificaciones.EnvioCorreoNotif(lstmodif, "erick2402199501@gmail.com", "M", "Proyectos");
            return Ok(resultado);
        }
    }
}
