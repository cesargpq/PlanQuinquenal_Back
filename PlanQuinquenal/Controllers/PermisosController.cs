using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly IRepositoryPermisos _repositoryPermisos;

        public PermisosController(IRepositoryPermisos repositoryPermisos)
        {
            _repositoryPermisos = repositoryPermisos;
        }

        [HttpGet("VisColumnTabla")]
        public async Task<IActionResult> VisColumnTabla(string correo, string tabla)
        {

            var resultado = await _repositoryPermisos.VisColumnTabla(correo, tabla);
            return Ok(resultado);
        }
    }
}
