using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Infrastructure.Data;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : ControllerBase
    {
        private readonly PlanQuinquenalContext _context;

        public AuditoriaController(PlanQuinquenalContext context)
        {
            this._context = context;
        }
        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var resultado = await _context.TablasAuditoria.Include(x => x.EventosAuditoria).ToListAsync();

            var cont = resultado.Count();
            List<EventosAuditoria> data = new List<EventosAuditoria>();
            EventosAuditoria obje = new EventosAuditoria();

            List<TablasAuditoria> data2 = new List<TablasAuditoria>();
            TablasAuditoria obje2 = new TablasAuditoria();


            foreach (var item in resultado)
            {
                obje2 = item;
                obje2.Estado = false;
                data2.Add(obje2);
                if(item.EventosAuditoria != null)
                {
                    foreach (var items in item.EventosAuditoria)
                    {
                        obje = items;
                        obje.Estado = false;
                        data.Add(obje);
                    }
                }
            }
            _context.UpdateRange(data2);
            _context.UpdateRange(data);

            await _context.SaveChangesAsync();
            return Ok(resultado);
        }
        
    }
}
