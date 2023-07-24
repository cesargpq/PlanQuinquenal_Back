using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Infrastructure.Data;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;
using PlanQuinquenal.Core.DTOs.RequestDTO;

namespace PlanQuinquenal.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : ControllerBase
    {
        private readonly PlanQuinquenalContext _context;
        private readonly ITrazabilidadRepository trazabilidadRepository;
        private readonly IAuthRepository authRepository;

        public AuditoriaController(PlanQuinquenalContext context, ITrazabilidadRepository trazabilidadRepository)
        {
            this._context = context;
            this.trazabilidadRepository = trazabilidadRepository;
            this.authRepository = authRepository;
        }
        [HttpPut]
        public async Task<IActionResult> Update(List<TablasAuditoria> t)
        {
            
            List<EventosAuditoria> data = new List<EventosAuditoria>();
            EventosAuditoria obje = new EventosAuditoria();

            List<TablasAuditoria> data2 = new List<TablasAuditoria>();
            TablasAuditoria obje2 = new TablasAuditoria();


            foreach (var item in t)
            {
                obje2 = item;
                obje2.Estado = item.Estado;
                data2.Add(obje2);
                if (item.EventosAuditoria != null)
                {
                    foreach (var items in item.EventosAuditoria)
                    {
                        obje = items;
                        obje.Estado = items.Estado;
                        data.Add(obje);
                    }
                }
            }
            _context.UpdateRange(data2);
            _context.UpdateRange(data);

            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var resultado = await _context.TablasAuditoria.Include(x => x.EventosAuditoria).ToListAsync();

            return Ok(resultado);
        }
        [HttpPost("Listar")]
        public async Task<IActionResult> Listar(RequestAuditDto r)
        {
            var resultado = await trazabilidadRepository.Listar(r);

            return Ok(resultado);
        }
    }
}
