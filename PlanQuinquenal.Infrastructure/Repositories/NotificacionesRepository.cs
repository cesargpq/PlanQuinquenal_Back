using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.Net.Mime;
using ApiDavis.Core.Utilidades;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class NotificacionesRepository : IRepositoryNotificaciones
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IConfiguration configuration;

        public NotificacionesRepository(PlanQuinquenalContext context, IServiceScopeFactory factory)
        {
            _context = context;
            configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        }

        public async Task<object> CambiarEstadoNotif(int cod_notif, bool flagVisto)
        {
            var modNotif = _context.Notificaciones.FirstOrDefault(p => p.id == cod_notif);
            modNotif.flag_visto = flagVisto;
            _context.SaveChanges();

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se modifico el estado de la notificacion"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;

        }

        public async Task<object> ContadorNotifNuevas(int cod_usu)
        {
            var queryable = _context.Notificaciones
                                     .Where(x => x.cod_usu == cod_usu)
                                     .Where(x => x.flag_visto == false)
                                     .AsQueryable();

            int cantidad = await queryable.CountAsync();
            var resp = new
            {
                cantidad = cantidad,
                idMensaje = "1",
                mensaje = "Se realizo la consulta correctamente correctamente\""
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public async Task<object> CrearConfigNotif(Config_notificaciones config)
        {
            try
            {
                var nuevaConfNotificacion = new Config_notificaciones
                {
                    cod_usu = config.cod_usu,
                    regPQ = config.regPQ,
                    modPQ = config.modPQ,
                    regPry = config.regPry,
                    modPry = config.modPry,
                    regImp = config.regImp,
                    modImp = config.modImp,
                    regPer = config.regPer,
                    modPer = config.modPer,
                    regEviReemp = config.regEviReemp,
                    modEviReemp = config.modEviReemp,
                    regCom = config.regCom,
                    modCom = config.modCom,
                    regInfActas = config.regInfActas,
                    modInfActas = config.modInfActas
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Config_notificaciones.Add(nuevaConfNotificacion);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creo la configuracion de la notificacion correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear la configuracion"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }

        public async Task<object> CrearNotificacion(Notificaciones notificacion)
        {
            try
            {
                // Crear una nueva instancia de la entidad
                var nuevaNotificacion = new Notificaciones
                {
                    cod_usu = notificacion.cod_usu,
                    seccion = notificacion.seccion,
                    nombreComp_usu = notificacion.nombreComp_usu,
                    cod_reg = notificacion.cod_reg,
                    area = notificacion.area,
                    fechora_not = notificacion.fechora_not,
                    flag_visto = notificacion.flag_visto,
                    tipo_accion = notificacion.tipo_accion,
                    mensaje = notificacion.mensaje,
                    codigo = notificacion.codigo,
                    modulo = notificacion.modulo
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Notificaciones.Add(nuevaNotificacion);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creo la notificacion correctamente",
                    codigoNot = nuevaNotificacion.id
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear la notificacion"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }

        public async Task<object> EnvioCorreoNotif(List<CorreoTabla> lstModif, string correoUsu, string tipoOperacion, string modulo)
        {
            try
            {
                // Configurar el correo electrónico
                string correoEnvioConf = configuration.GetSection("EmailSettings").GetSection("CorreoEnvio").Value;
                string smtp = configuration.GetSection("EmailSettings").GetSection("Smtp").Value;
                string port = configuration.GetSection("EmailSettings").GetSection("Port").Value;
                string destinatario = correoUsu;
                string asunto = "Notificacion de modficacion";
                string cuerpo = ConstruirCuerpoCorreo(lstModif, modulo, tipoOperacion);

                // Configurar el cliente SMTP
                SmtpClient clienteSmtp = new SmtpClient(smtp, Convert.ToInt32(port));
                clienteSmtp.Port = Convert.ToInt32(port);
                clienteSmtp.EnableSsl = false;
                clienteSmtp.UseDefaultCredentials = true;

                // Crear el correo

                MailMessage correo = new MailMessage();
                correo.From = new System.Net.Mail.MailAddress(correoEnvioConf);
                correo.To.Add(destinatario);
                correo.Subject = asunto;
                correo.IsBodyHtml = true;
                correo.Body = cuerpo;

                // Enviar el correo
                clienteSmtp.Send(correo);

                Console.WriteLine("Correo enviado exitosamente.");
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Correo enviado exitosamente.",
                    correoHtml = cuerpo
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;

            }
            catch(Exception ex)
            {
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = ex.Message
                };

                var json = JsonConvert.SerializeObject(resp); 
                return json;
            }
            
        }

        public async Task<object> ModificarConfigNotif(Config_notificacionesRequestDTO config, int codUsu)
        {
            try
            {
                var modNotif = _context.Config_notificaciones.FirstOrDefault(p => p.cod_usu == codUsu);
                modNotif.regPQ = config.regPQ;
                modNotif.modPQ = config.modPQ;
                modNotif.regPry = config.regPry;
                modNotif.modPry = config.modPry;
                modNotif.regImp = config.regImp;
                modNotif.modImp = config.modImp;
                modNotif.regPer = config.regPer;
                modNotif.modPer = config.modPer;
                modNotif.regEviReemp = config.regEviReemp;
                modNotif.modEviReemp = config.modEviReemp;
                modNotif.regCom = config.regCom;
                modNotif.modCom = config.modCom;
                modNotif.regInfActas = config.regInfActas;
                modNotif.modInfActas = config.modInfActas;
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico la configuracion de notificaciones correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al modificar las notificaciones"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<Config_notificaciones> ObtenerConfigNotif(int cod_usu)
        {
            Config_notificaciones config_Notificaciones= new Config_notificaciones();
            var queryCons = await _context.Config_notificaciones
                                     .Where(x => x.cod_usu == cod_usu)
                                     .ToListAsync();

            config_Notificaciones = queryCons[0];
            return config_Notificaciones;
        }

        public async Task<PaginacionResponseDto<Notificaciones>> ObtenerListaNotif(RequestNotificacionDTO r, int cod_usu)
        {
            //List<Notificaciones> lstNotificaciones = new List<Notificaciones>();
            var queryable =  _context.Notificaciones.Where(x => x.cod_usu == cod_usu).OrderByDescending(x => x.fechora_not).AsQueryable();
            int cantidad = queryable.Count();

            var dato = await queryable.Paginar(r).ToListAsync();

            var objeto = new PaginacionResponseDto<Notificaciones>
            {
                Cantidad = cantidad,
                Model = dato
            };
            return objeto;

            //return dato;
        }


        #region
        public static string ConstruirCuerpoCorreo(List<CorreoTabla> lstModif, string modulo, string tipoOperacion)
        {

            if (tipoOperacion == "M")
            {
                // Generar las filas de la tabla dinámicamente
                StringBuilder tablaHtml = new StringBuilder();
                foreach (var item in lstModif)
                {
                    tablaHtml.Append("<tr>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.codigo}</span>");
                        tablaHtml.Append("</td>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.campoModificado}</span>");
                        tablaHtml.Append("</td>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.valorActual}</span>");
                        tablaHtml.Append("</td>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.valorModificado}</span>");
                        tablaHtml.Append("</td>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.fechaMod}</span>");
                        tablaHtml.Append("</td>");
                        tablaHtml.Append("<td class='cuerpo'>");
                        tablaHtml.Append($"<span>{item.usuModif}</span>");
                        tablaHtml.Append("</td>");
                    tablaHtml.Append("</tr>");
                }

                // Combinar el HTML generado con la plantilla
                string html = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {
                            font-family: sans-serif;
                        }
                        .cabezera{
                            border: 1px solid #CCECF8;
                            background: #E3F4FA;
                            color: #00A1DE;
                            padding: 10px;
                        }
                        .cuerpo{
                            border: 1px solid #EBEBEB;
                            background: white;
                            color: #262626;
                            padding: 10px;
                        }
                    </style>
                </head>
                <body style='margin-top: 40px;margin: 0;'>
                    <div style='margin-left: 40px;margin-top: 60px;max-width: 800px;'>
                        <img src='https://quinquenalqas.calidda.com.pe/ProyectoQuinquenal/Images/logoCalidda.png' style=""left:0"" width=""220px"">
                    </div>
                    <div style='margin-left: 40px;max-width: 800px;margin-top: 60px;'>
                        <p style='color: #868685;font-weight: 300; line-height: 16.8px;'>Estimado Usuario,<br/><br/>
                        Se le informa que se ha realizado una modificación en la sección " + modulo+@", a continuación, se presenta el detalle:</p>
                    </div>
                    <table style='margin-left: 40px;margin-top: 30px;margin-bottom: 30px;max-width: 800px;border-spacing: 0px;'>
                        <thead>
                            <tr>
                                <th class='cabezera'>
                                    <span>Código</span>
                                </th>
                                <th class='cabezera'>
                                    <span>Campo modificado</span>
                                </th>
                                <th class='cabezera'>
                                    <span>Valor anterior</span>
                                </th>
                                <th class='cabezera'>
                                    <span>Valor modificado</span>
                                </th>
                                
                                <th class='cabezera'>
                                    <span>Fecha de modificación</span>
                                </th>
                                <th class='cabezera'>
                                    <span>Modificado por</span>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            "+tablaHtml+@"                            
                        </tbody>
                    </table>
                    <div style='background-color: #00A1DE;text-align: center;padding: 10px;max-width: 860px;'>
                        <div style='color: white;line-height: 24px;'>Cálidda <script>document.write(new Date().getFullYear())</script>. Todos los derechos reservados.</div>
                    </div>
                </body>
                </html>";


                return html;
            }
            else
            {
                string codigoCreacion = lstModif[0].codigo;
                string html = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {
                            font-family: sans-serif;
                        }
                        .cabezera{
                            border: 1px solid #CCECF8;
                            background: #E3F4FA;
                            color: #00A1DE;
                            padding: 10px;
                        }
                        .cuerpo{
                            border: 1px solid #EBEBEB;
                            background: white;
                            color: #262626;
                            padding: 10px;
                        }
                    </style>
                </head>
                <body style='margin-top: 40px;margin: 0;'>
                    <div style='margin-left: 40px;margin-top: 60px;max-width: 800px;'>
                        <img src='https://quinquenalqas.calidda.com.pe/ProyectoQuinquenal/Images/logoCalidda.png' style=""left:0"" width=""220px"">
                    </div>
                    <div style='margin-left: 40px;max-width: 800px;margin-top: 60px;'>
                        <p style='color: #868685;font-weight: 300; line-height: 16.8px;'>Estimado Usuario,<br/><br/>
                            Se le informa que se ha realizado una creación en la sección " + modulo+@" con el codigo "+codigoCreacion+@".</p>
                    </div>
                    <div style='background-color: #00A1DE;text-align: center;padding: 10px;max-width: 860px;margin-top: 60px;'>
                        <div style='color: white;line-height: 24px;'>Cálidda <script>document.write(new Date().getFullYear())</script>. Todos los derechos reservados.</div>
                    </div>
                </body>
                </html>";


                return html;
            }
            
        }

        public async Task<List<CorreoTabla>> ObtenerListaModif(int codNot)
        {
            var queryable = await _context.CorreoTabla.Where(x => x.idNotif == codNot).ToListAsync();

            return queryable;
        }
        #endregion
    }
}
