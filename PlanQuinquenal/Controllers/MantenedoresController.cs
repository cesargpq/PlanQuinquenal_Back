﻿    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MantenedoresController : ControllerBase
    {
        private readonly IRepositoryMantenedores _repositoryMantenedores;
        private readonly PlanQuinquenalContext _context;

        public MantenedoresController(IRepositoryMantenedores repositoryMantenedores, PlanQuinquenalContext context )
        {
            _repositoryMantenedores = repositoryMantenedores;
            this._context = context;
        }

        [HttpPost("ListarEntidad")]
        public async Task<IActionResult> GetAll(ListEntidadDTO entidad)
        {

            var resultado = await _repositoryMantenedores.GetAll(entidad);
            return Ok(resultado);
        }
        
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {

            var resultado = await _repositoryMantenedores.GetById(id);
            return Ok(resultado);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var resultado = await _repositoryMantenedores.DeleteById(id);
            ResponseDTO dto = new ResponseDTO();
            if (resultado)
            {
                dto.Valid = true;
                dto.Message = $"Se actualizó la entidad correctamente";
            }
            else
            {
                dto.Valid = false;
                dto.Message = $"Hubo un error al eliminar";
            }

            return Ok(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Post(PostEntityReqDTO postEntityReqDTO)
        {

            var resultado = await _repositoryMantenedores.Post(postEntityReqDTO);
            ResponseDTO dto = new ResponseDTO();
            if (resultado)
            {
                dto.Valid = true;
                dto.Message = $"Se creó el {postEntityReqDTO.Entidad} correctamente";
            }
            else
            {
                dto.Valid = false;
                dto.Message = $"Hubo un error al crear el {postEntityReqDTO.Entidad} correctamente";
            }

            return Ok(dto);
        }
        [HttpPut("id")]
        public async Task<IActionResult> Update(PostUpdateEntityDTO postEntityReqDTO,int id)
        {

            var resultado = await _repositoryMantenedores.Update(postEntityReqDTO,id);

            ResponseDTO dto = new ResponseDTO();
            if (resultado)
            {
                dto.Valid = true;
                dto.Message = $"Se actualizó la entidad correctamente";
            }
            else
            {
                dto.Valid = false;
                dto.Message = $"Hubo un error al actualizar";
            }

            return Ok(dto);
        }
    }
}
