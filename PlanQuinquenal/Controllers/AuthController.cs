using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
         
            this._authRepository = authRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Autenticar(LoginRequestDTO loginRequestDTO)
        {
            var jwtToken = await _authRepository.Autenticar(loginRequestDTO);
            return Ok(jwtToken);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims.ElementAt(1) != null)
            {
                var iModulo = identity.Claims.ElementAt(4);
                var data = identity.Claims.ElementAt(1);
                if (!data.Equals("CPAREDES1"))
                {
                    return Unauthorized();
                }
            }
            return Ok();
        }
    }
}
