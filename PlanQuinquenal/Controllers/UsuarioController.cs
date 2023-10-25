using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using PlanQuinquenal.Infrastructure.Repositories;

namespace PlanQuinquenal.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly PlanQuinquenalContext _context;
        private readonly Constantes _constante;

        public UsuarioController(IUsuarioRepository usuarioRepository, PlanQuinquenalContext context,Constantes constante)
        {
            this.usuarioRepository = usuarioRepository;
            this._context = context;
            this._constante = constante;
        }

        
        [HttpPost("Listar")]
        public async Task<IActionResult> GetAll(UsuarioListDTO usuarioListDTO)
        {
            var resultado = await usuarioRepository.GetAll(usuarioListDTO);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioRequestDto data)
        {
           
            var resultado = await usuarioRepository.CreateUser(data);
          

            return Ok(resultado);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var obj = new ResponseEntidadDto<Usuario>();
            var resultado = await usuarioRepository.GetById(id);
            if (resultado != null)
            {

                obj.Model = resultado;
                obj.Valid = true;
                obj.Message = "Registro encontrado satisfactoriamente";
                return Ok(obj);
            }
            else
            {
                obj.Valid = false;
                obj.Message = "No existe un baremo con el código solicfitado";
                return Ok(obj);
            }

        }
      

       
        [HttpPut("EditarUsuario")]
        public async Task<IActionResult> EditarUsuario(UsuarioRequestDto data, int id)
        {

            var resultado = await usuarioRepository.Update(data, id);

            return Ok(resultado);
            //return Ok(resultado);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {

            var resultado = await usuarioRepository.UpdateState(id);

            return Ok(resultado);
        }

        [HttpGet("correo")]
        public async Task<IActionResult> DesbloquearUsuario(string correo)
        {

            var resultado = await usuarioRepository.DesbloquearUsuario(correo);

            return Ok(resultado);
        }
    }
}
