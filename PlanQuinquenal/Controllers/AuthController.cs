using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            return Ok();
        }
    }
}
