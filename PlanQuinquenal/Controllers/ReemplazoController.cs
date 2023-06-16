using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReemplazoController : ControllerBase
    {
        public ReemplazoController()
        {
            
        }

        [HttpPost("ReemplazoImport")]
        public async Task<IActionResult> ReemplazoImport(RequestMasivo requestMasivo)
        {

            
            return Ok();    
        }
    }
}
