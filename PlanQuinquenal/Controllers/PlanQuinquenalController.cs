using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanQuinquenalController : ControllerBase
    {
        private readonly IPlanQuinquenalesRepository planQuinquenalesRepository;

        public PlanQuinquenalController(IPlanQuinquenalesRepository planQuinquenalesRepository)
        {
            this.planQuinquenalesRepository = planQuinquenalesRepository;
        }


        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(PQuinquenalRequestDTO p)
        {
            var resultado = await planQuinquenalesRepository.GetAll(p);
            return Ok(resultado);
        }
        [HttpPost]
        public async Task<IActionResult> Add(PQuinquenalReqDTO p)
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
            var resultado = await planQuinquenalesRepository.Add(p,idUser);
            return Ok(resultado);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await planQuinquenalesRepository.GetById(id);
            return Ok(resultado);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePlanQuinquenalDto dto,int id)
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
            var resultado = await planQuinquenalesRepository.Update(dto, id, idUser);
            return Ok(resultado);
        }
        //[HttpPost]
        //public async Task<IActionResult> CreatePQ(PQuinquenalReqDTO pQuinquenalReqDTO)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.CreatePQ(pQuinquenalReqDTO, idUser);
        //    return Ok(resultado);
        //}

        //[HttpPost("CrearComentario")]
        //public async Task<IActionResult> CrearComentario(Comentarios_proyecDTO comentario)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.CrearComentario(comentario, idUser, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("ActualizarPQ")]
        //public async Task<IActionResult> ActualizarPQ(PQuinquenalReqDTO planquinquenal)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.ActualizarPQ(planquinquenal, idUser);
        //    return Ok(resultado);
        //}

        //[HttpDelete("EliminarComentario")]
        //public async Task<IActionResult> EliminarComentario(int codigo)
        //{

        //    var resultado = await planQuinquenalesRepository.EliminarComentario(codigo, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("CrearDocumento")]
        //public async Task<IActionResult> CrearDocumento(Docum_proyectoDTO requestDoc)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.CrearDocumento(requestDoc, idUser, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpDelete("EliminarDocumento")]
        //public async Task<IActionResult> EliminarDocumento(int codigo)
        //{

        //    var resultado = await planQuinquenalesRepository.EliminarDocumento(codigo, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("CrearInforme")]
        //public async Task<IActionResult> CrearInforme(InformeRequestDTO requestDoc)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.CrearInforme(requestDoc, idUser, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("ModificarInforme")]
        //public async Task<IActionResult> ModificarInforme(InformeRequestDTO requestDoc)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.ModificarInforme(requestDoc, idUser, "PQ");
        //    return Ok(resultado);
        //}
        //[HttpDelete("EliminarInforme")]
        //public async Task<IActionResult> EliminarInforme(int codigo)
        //{

        //    var resultado = await planQuinquenalesRepository.EliminarInforme(codigo, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("CrearActa")]
        //public async Task<IActionResult> CrearActa(ActaRequestDTO requestDoc)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.CrearActa(requestDoc, idUser, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpPost("ModificarActa")]
        //public async Task<IActionResult> ModificarActa(ActaRequestDTO requestDoc)
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    var idUser = 0;
        //    foreach (var item in identity.Claims)
        //    {
        //        if (item.Type.Equals("$I$Us$@I@D"))
        //        {
        //            idUser = Convert.ToInt16(item.Value);

        //        }
        //    }
        //    var resultado = await planQuinquenalesRepository.ModificarActa(requestDoc, idUser, "PQ");
        //    return Ok(resultado);
        //}

        //[HttpDelete("EliminarActa")]
        //public async Task<IActionResult> EliminarActa(int codigo)
        //{

        //    var resultado = await planQuinquenalesRepository.EliminarInforme(codigo, "PQ");
        //    return Ok(resultado);
        //}
    }
}
