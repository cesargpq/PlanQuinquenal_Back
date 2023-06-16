using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Filters;
using PlanQuinquenal.Infrastructure.Repositories;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    //S[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectosController : ControllerBase
    {
        private readonly IRepositoryProyecto _repositoryProyecto;

        public ProyectosController(IRepositoryProyecto repositoryProyecto)
        {
            _repositoryProyecto = repositoryProyecto;
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

        [HttpPost("ProyectoImport")]
        public async Task<IActionResult> ProyectoImport(RequestMasivo data)
        {

            var resultado = await _repositoryProyecto.ProyectoImport(data);


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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var idUser = 0;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUser = Convert.ToInt16(item.Value);

                }
            }

            var resultado = await _repositoryProyecto.ObtenerProyectoxNro(nroProy, idUser);

            return Ok(resultado);
        }

        [HttpPost("CrearComentario")]
        public async Task<IActionResult> CrearComentario(Comentarios_proyecDTO comentario)
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
            var resultado = await _repositoryProyecto.CrearComentario(comentario, idUser , "P");
            return Ok(resultado);
        }

        [HttpPost("EliminarComentario")]
        public async Task<IActionResult> EliminarComentario(int codigo)
        {

            var resultado = await _repositoryProyecto.EliminarComentario(codigo, "P");
            return Ok(resultado);
        }

        [HttpPost("CrearDocumento")]
        public async Task<IActionResult> CrearDocumento(Docum_proyectoDTO requestDoc)
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
            var resultado = await _repositoryProyecto.CrearDocumento(requestDoc, idUser, "P");
            return Ok(resultado);
        }

        [HttpPost("EliminarDocumento")]
        public async Task<IActionResult> EliminarDocumento(int codigo)
        {

            var resultado = await _repositoryProyecto.EliminarDocumento(codigo, "P");
            return Ok(resultado);
        }

        [HttpPost("CrearPermiso")]
        public async Task<IActionResult> CrearPermiso(Permisos_proyecDTO requestDoc)
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
            var resultado = await _repositoryProyecto.CrearPermiso(requestDoc, idUser, "P");
            return Ok(resultado);
        }

        [HttpPost("EliminarPermiso")]
        public async Task<IActionResult> EliminarPermiso(int codigo)
        {

            var resultado = await _repositoryProyecto.EliminarPermiso(codigo, "P");
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
            var resultado = await _repositoryProyecto.CrearInforme(requestDoc, idUser, "P");
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
            var resultado = await _repositoryProyecto.ModificarInforme(requestDoc, idUser, "P");
            return Ok(resultado);
        }
        [HttpPost("EliminarInforme")]
        public async Task<IActionResult> EliminarInforme(int codigo)
        {

            var resultado = await _repositoryProyecto.EliminarInforme(codigo, "P");
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
            var resultado = await _repositoryProyecto.CrearActa(requestDoc,idUser, "P");
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
            var resultado = await _repositoryProyecto.ModificarActa(requestDoc, idUser, "P"); 
            return Ok(resultado);
        }
        [HttpPost("EliminarActa")]
        public async Task<IActionResult> EliminarActa(int codigo)
        {

            var resultado = await _repositoryProyecto.EliminarInforme(codigo, "P");
            return Ok(resultado);
        }

        [HttpPost("CrearDocumentoPr")]
        public async Task<IActionResult> CrearDocumentoPr(DocumentoProyRequest requestDoc)
        {

            var resultado = await _repositoryProyecto.CrearDocumentoPr(requestDoc, "P");
            return Ok(resultado);
        }
    }
}
