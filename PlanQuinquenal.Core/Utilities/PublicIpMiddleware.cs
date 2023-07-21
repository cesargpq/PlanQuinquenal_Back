using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace PlanQuinquenal.Core.Utilities
{
    public class PublicIpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;

        public PublicIpMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
        }
        public async Task InvokeAsync(HttpContext context)
        {

            var publicIP = context.Connection.RemoteIpAddress;

            // Si estás detrás de un proxy o equilibrador de carga, verifica el encabezado X-Forwarded-For.
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                publicIP = IPAddress.Parse(context.Request.Headers["X-Forwarded-For"].ToString());
            }

            context.Items["PublicIP"] = publicIP;

            await _next(context);
          
        }

    }
}
