using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;

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

            var resultado = await baremoRepository.CrearBaremo(data);


            return Ok(resultado);
        }

        [HttpPost("BaremoImport")]
        public async Task<IActionResult> BaremoImport(ProyectoRequest data)
        {
            
            var resultado = await baremoRepository.BaremoImport(data);

           
            return Ok(resultado);
        }
        [HttpPut("Editarbaremo")]
        public async Task<IActionResult> Editarbaremo(BaremoRequestDto data, int id)
        {

            var resultado = await baremoRepository.Editarbaremo(data, id);


            return Ok(resultado);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> EliminarBaremo(int id)
        {

            var resultado = await baremoRepository.EliminarBaremo(id);


            return Ok(resultado);
        }
      
    }
}
