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

namespace PlanQuinquenal.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : ControllerBase
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IAuthRepository authRepository;

        public AuditoriaController(PlanQuinquenalContext context, IAuthRepository authRepository)
        {
            this._context = context;
            this.authRepository = authRepository;
        }
        [HttpGet]
        public async Task<IActionResult>  Get()
        {
            var publicIP = HttpContext.Items["PublicIP"] as IPAddress;

            
            //var resultado = await _context.TablasAuditoria.Include(x => x.EventosAuditoria).ToListAsync();

            //var cont = resultado.Count();
            //List<EventosAuditoria> data = new List<EventosAuditoria>();
            //EventosAuditoria obje = new EventosAuditoria();

            //List<TablasAuditoria> data2 = new List<TablasAuditoria>();
            //TablasAuditoria obje2 = new TablasAuditoria();


            //foreach (var item in resultado)
            //{
            //    obje2 = item;
            //    obje2.Estado = false;
            //    data2.Add(obje2);
            //    if(item.EventosAuditoria != null)
            //    {
            //        foreach (var items in item.EventosAuditoria)
            //        {
            //            obje = items;
            //            obje.Estado = false;
            //            data.Add(obje);
            //        }
            //    }
            //}
            //_context.UpdateRange(data2);
            //_context.UpdateRange(data);

            //await _context.SaveChangesAsync();
            return Ok();
        }
        
    }
}
