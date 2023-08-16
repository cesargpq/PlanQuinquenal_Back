using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IRepositoryLogin _repositoryLogin;
        private readonly IAuthRepository _authRepository;
        public LoginController(IRepositoryLogin repositoryLogin, IAuthRepository authRepository)
        {
            _repositoryLogin = repositoryLogin;
            this._authRepository = authRepository;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Autenticar(LoginRequestDTO loginRequestDTO)
        {
            var jwtToken = await _authRepository.Autenticar(loginRequestDTO);

            return Ok(jwtToken);
        }
        [AllowAnonymous]
        [HttpPut("CerrarSesion")]
        public async Task<IActionResult> CerrarSesion(LoginRequestDTO login)
        {
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            //int idUser = 0;
            ResponseDTO rdto = new ResponseDTO();
            //if (identity!= null)
            //{

            //    foreach (var item in identity.Claims)
            //    {
            //        if (item.Type.Equals("$I$Us$@I@D"))
            //        {
            //            idUser = Convert.ToInt16(item.Value);
            //            break;
            //        }
            //    }
            //}


            var userResponse = await _authRepository.CerrarSesion(login);
            if(!userResponse)
            {
                rdto.Valid = false;
                rdto.Message = "El Usuario no se encuentra loggeado o no existe";
                return Ok(rdto);
            }
            rdto.Valid = true;
            rdto.Message = "Se desconectó al usuario satisfactoriamente";
            return Ok(rdto);
        }


        

        [HttpPost("AutenticarUsuario")]
        public async Task<IActionResult> AutenticarUsuario()
        {

            var resultado = ""; //await _repositoryLogin.Post(reqLogin);
            return Ok(resultado);
        }


        [HttpGet("ObtenerModulos")]
        public async Task<IActionResult> GetById()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims.ElementAt(1) != null)
            {
                var correo = identity.Claims.ElementAt(1).Value;
                var resultado = await _repositoryLogin.ObtenerModulos(correo);
                return Ok(resultado);
            }
            else
            {
                ModulosResponse resp =  new ModulosResponse();
                resp.idMensaje = "0";
                resp.mensaje = "Hubo un error al momento de obtener su informacion";
                return Ok(resp);
            }

            
        }
        [AllowAnonymous]
        [HttpPost("VerificaDobleFactor")]
        public async Task<IActionResult> VerificaDobleFactor(DFactorDTO dFactorDTO)
        {

            var stream = dFactorDTO.Token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var idUser = 0;
            foreach (var item in tokenS.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUser = Convert.ToInt16(item.Value);
                }
                
            }
        
            
            ResponseDTO rp = new ResponseDTO();
            var resultado = await _repositoryLogin.VerificaDobleFactor(dFactorDTO, idUser);
            if (resultado)
            {
                rp.Valid = true;
                rp.Message = "Doble Factor verificado";
            }
            else
            {
                rp.Valid = false;
                rp.Message = "Doble Factor no verificado";

            }
            return Ok(rp);
        }

        [HttpGet("ObtenerSeccionesMod")]
        public async Task<IActionResult> ObtenerSecciones(string modulo, string seccion)
        {

            var resultado = await _repositoryLogin.ObtenerSecciones(modulo, seccion);
            return Ok(resultado);
        }
    }
}
