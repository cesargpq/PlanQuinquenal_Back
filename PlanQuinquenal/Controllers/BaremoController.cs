using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Repositories;
using System.Net;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaremoController : ControllerBase
    {
        private readonly IBaremoRepository baremoRepository;

        public BaremoController(IBaremoRepository baremoRepository)
        {
            this.baremoRepository = baremoRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CrearBaremo(BaremoRequestDto data)
        {
            var ExisteQnq = await baremoRepository.ExisteQuinquenal(data.PlanQuinquenalId);

            ResponseDTO dto =new  ResponseDTO();
            if (ExisteQnq)
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
                var resultado = await baremoRepository.CrearBaremo(data,usuario);
                return Ok(resultado);
            }
            else
            {
                dto.Valid = false;
                dto.Message = Constantes.NoExistePQNQ;
                return Ok(dto);
            }
            


       
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var obj = new ResponseEntidadDto<Baremo>();
            var resultado = await baremoRepository.GetById(id);
            if (resultado != null)
            {

                obj.Model = resultado;
                obj.Valid = true;
                obj.Message = Constantes.RegistroExiste;
                return Ok(obj);
            }
            else
            {
                obj.Valid = false;
                obj.Message = Constantes.BaremoNoExiste;
                return Ok(obj);
            }
          
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> GetAll(BaremoListDto baremoListDto)
        {
            var resultado = await baremoRepository.GetAll(baremoListDto);
            
            return Ok(resultado);
        }

        [HttpPost("BaremoImport")]
        public async Task<IActionResult> BaremoImport(RequestMasivo data)
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
            var resultado = await baremoRepository.BaremoImport(data,usuario);

           
            return Ok(resultado);
        }
        [HttpPut("Editarbaremo")]
        public async Task<IActionResult> Editarbaremo(BaremoRequestDto data, int id)
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
            var resultado = await baremoRepository.Editarbaremo(data, id, usuario);


            return Ok(resultado);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> EliminarBaremo(int id)
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
            var resultado = await baremoRepository.EliminarBaremo(id,usuario);


            return Ok(resultado);
        }
      
    }
}
