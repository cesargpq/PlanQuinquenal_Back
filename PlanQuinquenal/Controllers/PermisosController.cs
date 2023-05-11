﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> VisColumnTabla(string tabla)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims.ElementAt(1) != null)
            {
                var correo = identity.Claims.ElementAt(1).Value;
                var resultado = await _repositoryPermisos.VisColumnTabla(correo, tabla);
                return Ok(resultado);
            }
            else
            {
                ColumsTablaPerResponse resp = new ColumsTablaPerResponse();
                resp.idMensaje = "0";
                resp.mensaje = "Hubo un error al momento de obtener su informacion";
                return Ok(resp);
            }

            
        }

        [HttpPost("ActualizarPermisosMod")]
        public async Task<IActionResult> ActualizarPermisosMod(TablaPerm_viz_modulo reqModulos)
        {
            var resultado = await _repositoryPermisos.ActualizarPermisosMod(reqModulos);
            return Ok(resultado);
        }

        [HttpPost("ActAccionesPerfil")]
        public async Task<IActionResult> ActAccionesPerfil(List<Acciones_Rol> reqAccRol)
        {
            var resultado = await _repositoryPermisos.ActAccionesPerfil(reqAccRol);
            return Ok(resultado);
        }

        [HttpGet("ConsAccionesPerfil")]
        public async Task<IActionResult> ConsAccionesPerfil()
        {
            var resultado = await _repositoryPermisos.ConsAccionesPerfil();
            return Ok(resultado);
        }
    }
}
