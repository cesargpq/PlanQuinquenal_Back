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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class NotificacionesRepository : IRepositoryNotificaciones
    {
        private readonly PlanQuinquenalContext _context;

        public NotificacionesRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }

        public async Task<object> CambiarEstadoNotif(int cod_notif, bool flagVisto)
        {
            var modNotif = _context.Notificaciones.FirstOrDefault(p => p.id == cod_notif);
            modNotif.flag_visto = flagVisto;
            _context.SaveChanges();

            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se modifico el proyecto correctamente"
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
                mensaje = "Se modifico el proyecto correctamente"
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
                    mensaje = notificacion.mensaje
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Notificaciones.Add(nuevaNotificacion);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creo la notificacion correctamente"
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

        public async Task<object> EnvioCorreoNotif(Object lst1, Object lst2, int cod_usu)
        {
            try
            {
                // Configurar el correo electrónico
                string remitente = "erick2402199501@gmail.com";
                string destinatario = "erick2402199501@gmail.com";
                string asunto = "Correo con diseño personalizado";
                string cuerpo = ConstruirCuerpoCorreo();

                // Configurar el cliente SMTP
                SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587);
                clienteSmtp.EnableSsl = true;
                clienteSmtp.UseDefaultCredentials = false;
                clienteSmtp.Credentials = new NetworkCredential("erick2402199501@gmail.com", "aafobhjkuqaogadq");

                // Crear el correo
                
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(remitente);
                correo.To.Add(destinatario);
                correo.Subject= asunto;
                correo.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpo, new ContentType("text/html"));
                correo.AlternateViews.Add(htmlView);

                // Enviar el correo
                clienteSmtp.Send(correo);

                Console.WriteLine("Correo enviado exitosamente.");
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "es una prueba"
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

        public async Task<object> ModificarConfigNotif(Config_notificaciones config)
        {
            try
            {
                var modNotif = _context.Config_notificaciones.FirstOrDefault(p => p.id == config.id);
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

        public async Task<List<Notificaciones>> ObtenerListaNotif(int cod_usu)
        {
            List<Notificaciones> lstNotificaciones = new List<Notificaciones>();
            var queryable = await _context.Notificaciones.Where(x => x.cod_usu == cod_usu).ToListAsync();

            return queryable;
        }


        #region
        public static string ConstruirCuerpoCorreo()
        {
            // Leer el archivo CSS
            string css = File.ReadAllText("Correo/main.css");

            // Leer el archivo de imagen
            string imagePath = "Correo/v270_5566.png";
            string imagenBase64 = "";
            if (File.Exists(imagePath))
            {
                // Convertir la imagen a base64
                byte[] imagenBytes = File.ReadAllBytes(imagePath); 
                imagenBase64 = Convert.ToBase64String(imagenBytes);
            }

            // URL del enlace externo para la fuente
            string fontLink = "https://fonts.googleapis.com/css?family=Open+Sans&display=swap";
            string html = @"
                <html>
                <head>
                <link href=""{0}"" rel=""stylesheet"">
                <style>{1}</style>
                </head>
                <body> 
                   <div class=""v270_5541"">
                        <img src=""https://www.calidda.com.pe/media/ohen2u1d/logo.png"" alt=""Mi imagen"" />
                   </div>
		        <div style=""width: 600px;height: 70px;"">
		        <span class=""v270_5542"">Estimado Usuario,
                Se le informa que se ha realizado una modificación en la sección Proyectos, a continuación, se presenta el detalle:</span>
		        </div>
         
                <div style=""margin-bottom: 30px;"">
		            <div class=""v270_5543"">
			        <span class=""v270_5550"">Código de proyecto o PQ</span>
		            </div>
                    <div class=""v270_5546"">
			        <span class=""v270_5551"">Valor modificado</span>
		            </div>
                    <div class=""v270_5547"">
			        <span class=""v270_5552"">Campo modificado</span>
		            </div>
                    <div class=""v270_5548"">
			        <span class=""v270_5553"">Fecha de modificación</span>
		            </div>
                    <div class=""v270_5549"">
			        <span class=""v270_5554"">Modificado por</span>
		            </div>
		 
                    <div class=""v270_5556"">
			        <span class=""v270_5561"">PPE0-21-0304</span>
		            </div>
                    <div class=""v270_5557"">
			        <span class=""v270_5562"">120</span>
		            </div>
                    <div class=""v270_5558"">
			        <span class=""v270_5563"">LONG. REAL PEND.</span>
		            </div>
                    <div class=""v270_5559"">
			        <span class=""v270_5564"">31/03/2023</span>
		            </div>
                    <div class=""v270_5560"">
			        <span class=""v270_5565"">Juan Pérez</span>
		            </div>
                </div>
         
		        <div class=""v270_5544"">
			    <span class=""v270_5545"">Cálidda 2023. Todos los derechos reservados.</span>
		        </div>
                </body>
                </html>";
            html = string.Format(html, fontLink, css);


            return html;
        }
        #endregion
    }
}
