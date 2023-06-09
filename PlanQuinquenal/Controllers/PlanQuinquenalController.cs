using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanQuinquenalController : ControllerBase
    {
        private readonly IPlanQuinquenalesRepository planQuinquenalesRepository;

        public PlanQuinquenalController(IPlanQuinquenalesRepository planQuinquenalesRepository)
        {
            this.planQuinquenalesRepository = planQuinquenalesRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resultado = await planQuinquenalesRepository.Get();
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePQ(PQuinquenalReqDTO pQuinquenalReqDTO)
        {
            var resultado = await planQuinquenalesRepository.CreatePQ(pQuinquenalReqDTO);
            return Ok(resultado);
        }
    }
}
