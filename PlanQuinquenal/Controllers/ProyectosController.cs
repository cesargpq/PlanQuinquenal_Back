using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Filters;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly IRepositoryProyecto _repositoryProyecto;
        private readonly IRepositoryMetodosRehusables _repositoryMetodosRehusables;

        public ProyectosController(IRepositoryProyecto repositoryProyecto, IRepositoryMetodosRehusables repositoryMetodosRehusables)
        {
            _repositoryProyecto = repositoryProyecto;
            _repositoryMetodosRehusables = repositoryMetodosRehusables;
        }

        [HttpPost("NuevoProyecto")]
        public async Task<IActionResult> NuevoProyecto(ProyectoRequest nvoProyecto)
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

            var resultado = await _repositoryProyecto.NuevoProyecto(nvoProyecto, idUser);
            return Ok(resultado);
        }

        [HttpPost("ObtenerProyectos")]
        public async Task<IActionResult> ObtenerProyectos(FiltersProyectos filterProyectos)
        {

            var resultado = await _repositoryProyecto.ObtenerProyectos(filterProyectos);
            return Ok(resultado);
        }

        [HttpPost("NuevosProyectosMasivo")]
        public async Task<IActionResult> NuevosProyectosMasivo(ProyectoRequest reqMasivo)
        {

            var resultado = await _repositoryProyecto.NuevosProyectosMasivo(reqMasivo);
            return Ok(resultado);
        }

        [HttpPost("ActualizarProyecto")]
        public async Task<IActionResult> ActualizarProyecto(ProyectoRequest nvoProyecto)
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
            var resultado = await _repositoryProyecto.ActualizarProyecto(nvoProyecto, idUser);
            return Ok(resultado);
        }

        [HttpGet("ObtenerProyectoxNro")]
        public async Task<IActionResult> ObtenerProyectoxNro(int nroProy)
        {

            var resultado = await _repositoryProyecto.ObtenerProyectoxNro(nroProy);

            return Ok(resultado);
        }

        [HttpPost("CrearComentario")]
        public async Task<IActionResult> CrearComentario(Comentarios_proyec comentario)
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
            var resultado = await _repositoryMetodosRehusables.CrearComentario(comentario, idUser);
            return Ok(resultado);
        }

        [HttpPost("EliminarComentario")]
        public async Task<IActionResult> EliminarComentario(int codigo)
        {

            var resultado = await _repositoryMetodosRehusables.EliminarComentario(codigo);
            return Ok(resultado);
        }

        [HttpPost("CrearDocumento")]
        public async Task<IActionResult> CrearDocumento(Docum_proyecto requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.CrearDocumento(requestDoc, idUser);
            return Ok(resultado);
        }

        [HttpPost("EliminarDocumento")]
        public async Task<IActionResult> EliminarDocumento(int codigo)
        {

            var resultado = await _repositoryMetodosRehusables.EliminarDocumento(codigo);
            return Ok(resultado);
        }

        [HttpPost("CrearPermiso")]
        public async Task<IActionResult> CrearPermiso(Permisos_proyec requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.CrearPermiso(requestDoc, idUser);
            return Ok(resultado);
        }

        [HttpPost("EliminarPermiso")]
        public async Task<IActionResult> EliminarPermiso(int codigo)
        {

            var resultado = await _repositoryMetodosRehusables.EliminarPermiso(codigo);
            return Ok(resultado);
        }

        [HttpPost("CrearInforme")]
        public async Task<IActionResult> CrearInforme(InformeRequestDTO requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.CrearInforme(requestDoc, idUser);
            return Ok(resultado);
        }

        [HttpPost("ModificarInforme")]
        public async Task<IActionResult> ModificarInforme(InformeRequestDTO requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.ModificarInforme(requestDoc, idUser);
            return Ok(resultado);
        }
        [HttpPost("EliminarInforme")]
        public async Task<IActionResult> EliminarInforme(int codigo)
        {

            var resultado = await _repositoryMetodosRehusables.EliminarInforme(codigo);
            return Ok(resultado);
        }

        [HttpPost("CrearActa")]
        public async Task<IActionResult> CrearActa(ActaRequestDTO requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.CrearActa(requestDoc,idUser);
            return Ok(resultado);
        }

        [HttpPost("ModificarActa")]
        public async Task<IActionResult> ModificarActa(ActaRequestDTO requestDoc)
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
            var resultado = await _repositoryMetodosRehusables.ModificarActa(requestDoc, idUser); 
            return Ok(resultado);
        }
        [HttpPost("EliminarActa")]
        public async Task<IActionResult> EliminarActa(int codigo)
        {

            var resultado = await _repositoryMetodosRehusables.EliminarInforme(codigo);
            return Ok(resultado);
        }

        [HttpPost("CrearDocumento")]
        public async Task<IActionResult> CrearDocumento(DocumentoProyRequest requestDoc)
        {

            var resultado = await _repositoryProyecto.CrearDocumento(requestDoc);
            return Ok(resultado);
        }
    }
}
