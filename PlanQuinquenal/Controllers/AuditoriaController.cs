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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int idUsuario = 0;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUsuario = Convert.ToInt16(item.Value);
                }
            }

            var datazz = await authRepository.GetToken(idUsuario);

            if (!datazz)
            {
                return Unauthorized();
            }
            
            Logs log = new Logs();
            string ipAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            if (ipAddress == "::1")
            {
                var datas = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[5].ToString() + "-" + Dns.GetHostEntry(Dns.GetHostName()).AddressList[6].ToString();
            }

            log.servicio = ipAddress;

            
            _context.Add(log);
            await _context.SaveChangesAsync();
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
