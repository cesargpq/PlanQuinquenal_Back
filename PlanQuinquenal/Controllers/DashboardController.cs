using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        public DashboardController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> Listar()
        {

            return Ok();
        }
    }
}
