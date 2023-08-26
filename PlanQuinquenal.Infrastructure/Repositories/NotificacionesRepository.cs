﻿using Microsoft.EntityFrameworkCore;
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
                string asunto = "Notificación de modificación";
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
                        <img style='zoom: 1.5;' width='89' height='75' src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAUwAAAEWCAYAAAANe67OAAAgAElEQVR4Ae2dBZgcx5n3FTrnwoktmSmmxEkccNCO7cQOXZK7JJdL7u67S3KxY5Jk2TIzM8ooMrO0klasFaykJTEzM8MOw+L7Pb/qrtne0UDvbs/szGz189T27ExD1VvV/3757SVmMxQwFDAUMBRwRYFero4yBxkKGAoYChgKiAFMswgMBQwFDAVcUsAApktCmcMMBQwFDAUMYJo1YChgKGAo4JICBjBdEsocZihgKGAoYADTrAFDAUMBQwGXFDCA6ZJQ5jBDAUMBQwEDmGYNGAoYChgKuKSAAUyXhDKHGQoYChgKGMA0a8BQwFDAUMAlBQxguiSUOcxQwFDAUMAAplkDhgKGAoYCLilgANMlocxhhgKGAoYCBjDNGjAUMBQwFHBJAQOYLgllDjMUMBQwFDCAadaAoYChgKGASwoYwHRJKHOYoYChgKGAAUyzBgwFDAUMBVxSwACmS0KZwwwFDAUMBQxgmjVgKGAoYCjgkgIGMF0SyhxmKGAoYChgANOsAUMBQwFDAZcUMIDpklDmMEMBQwFDAQOYZg0YChgKGAq4pIABTJeEMocZChgKGAoYwDRrwFDAUMBQwCUFDGC6JJQ5zFDAUMBQwACmWQOGAoYChgIuKWAA0yWhzGGGAoYChgIGMM0ayCkFfI2tOb2+ubihQD4pYAAzn9TuYfdqbhX5++KIvLQ53sNGboZbqhQwgFmqM1sA4xq/t1F6ldXLx8f45LlNBjQLYEpMF7pIAQOYXSSgOT01BRpaRC6rCUuvkfXSa5RPPjraL4+u73mguSXcIjP2N8n+uFFNpF4pxfWtAczimq+i6W35nkb52Bif9Brtk17sFWj65JF1saIZQ2c6ihpidbBFhm+Ny/8sjMiZ0wLy2fF+mbKvsTOXM+cUGAUMYBbYhJRCdwCN384JWdwlYKnbKJ8C0Sc3lB6nuTXSIq9ta5A/zo/ICZMD1osC7po22ifv7mgohant8WMwgNnjl4D3BJh9sEk+NdavuMoEWDpA85/KffLyluIHzWizyKwDTdJvWVTOmBqwXgy2CiLBWcNhj/LJy8bw5f1C64YrGsDsBqKX+i2vXBI5krvUgMm+zCefGeeT93cWJ9d1sKFVcZO/qA3LZ8b5lWEL41YCJJ1jtQHzqQ2lrYoo9TWtx2cAU1PC7D2hwMZwi5w4JQ136QSSsno5dqJfZh5o8uS++bjItkiLPLkhJt+qDFrcs+YmneNK9Xlkvdy/xgBmPuYo1/cwgJlrCvew6w/aGJdeo9JwW8lgMrJezp4WkDXBloKm0pZIizywNiZnInaXwSG7HJ8e78h6uWN1tKDHaDrnjgIGMN3RyRzlggKWK1HIAhQNFtn2I+rl4uqQIOYW2rYrZgHlqVMAyvqOA6UeuwHMQpvaTvfHAGanSWdOTKZA7aEm+dwEF+K4BhL26PhG1quIIKzrhbAB3k9tiMtZ0wKWLhawHOOTjzjaEcYs55iSP4+ol9tWGg6zEOa2q30wgNlVCprzExS4Z03MElmTASPb/4BmmU+e3ti9lvPGFpH3djTIt9FRApKoFhwg6QRMPrsGTQBzlQHMxEIp4g8GMIt48gqp64HGVvnRbHfieDLwKPAp88lnx/lk2v7uMQJVH2qS384JWyAIWJb75KN2+0h5e+4yuf9ZgXNkvdy92hh9Cmm9drYvBjA7SzlzXjsKVB1qks+MTy+OJ4NMqv8Rzc+dHpBd0fwZgXbHWuTWVVH5HH23Re+PlfuEpgGTfZdAc2S9Mhq1I5j5pygpYACzKKet8DpNyKMGnFRgmPE7ByABmv+7KCItedBnjtjVKN9ULkKW1VsDpd47AVN/zgScaTnNMp88tt5wmIW3ajveIwOYHaeZOSOJArGWVvlNXbhjgGmDpAYivVegM7pehm/NnVM7YYxXLY3IP40lCqdePjbWJx+nlVtNA6be677pfSbQ5MXQDjhtx/UXTaRP0qopzn8NYBbnvBVUr9cFW6z46bL6dpZkxVWmAUYNPsl7QAoQO3FKQCWx8Hqgo3c3ynm24znA94mxbS0noAlgjvHJG9tz9wLwmkbmeukpYAAzPW3MLy4pQGIJuDNchJIB0O3/mpvTe8T7P84LC76dXmyHGlrl9tVR+cx4KzHGJ8b65Z/G+tsBJuCpQDMNt5lqLIrb5KWQxpoOTRhT2S4DmF7MY3dfwwBmd89ACdz/hhVRxRUCHsmgogEw216Lw3oPAH10jE/FbHeVREv8zfJLVAZcs9wvR41ra4Cm1do4zQRwuhTRGXM6MZ3EG58a55fKIgoB7Sq9S/l8A5ilPLt5GBs1ey6pDinA1GCZDRydv2uATOwdXF6v0fVy9vSAEJrY2W3krkY5a3pQgSVA+c/j/PJJB2AelQDMIzlOJ7fp7DOf9ViT98mcJoD5pQl+Werv/Bg6O3ZznvcUMIDpPU171BVXBZrluMl+JY4ngwogqL9LBYgAUoKbc+gS+U5/D+D0X9Zxp2+ihojW+QKRR+V++efxfvnU+DbABDTbAec4zWlmFtP1eJz7I0DTIaLT/+MmBWRT2ABmKTwYBjBLYRa7cQwYUZS1eYwFjglgtMXZBJfm4Bw1IKbacy2rWQD2kTF++eIEv1QdanY9ynBTq6AmOGocOkm/fNoGSwWYLkGTPjj7lxiH4yXgBjR7ldXLOdOCsieWBz8p1xQyB3aWAgYwO0s5c56iwL1rLP0lXFYCVGxwdAJOqs9OYGzTJfrlqLF2g+sj3+Ron/xhXkTiLpi0w42t8peFYSUyf3K8Xz47wa8c6gFN18DpENOd/U6Mz6VuE70mgElykUCTAcxSeGQMYJbCLHbTGICAvyyKKP0lYOIEl+TPbZwjHGSb+MvnBEDy2alftMXmjyNSj/PLmD2Z6+IcbmiV388Ly0dsrpJEIAowOwOaaUT0joImLlL/uSAsBi+7aZF6fFsDmB4TtCddjqw+F1YFBeMMAJmOY0yAYhIYanDU+sRUe4CSBpf5q7qwhNOkNMJt6N/nh5W1+nMT/fL5iX6VOQnQTAWciOe6od/U99F90H07ygHuzpeAG+DUHOaNJlNRyTwWBjBLZirzP5B1wWY5mVyRY9pzjYBMoqUASQ1KyXsNWok9QGYDG6I5AFe2+0guM9TUKv9vUVQ+OtYCyi/YgAloauCE0+wst9nGEbfnorOBpgLMUfXK+JT/2TF3zAUFDGDmgqo95Jo1h5rlCxMCR/g2wp0lg6Hz/wQg2tyj+t/B8WnOT++1/hFg/v3csEQd9h9E3YErYkqU/8KkgHxpUkC+OCkggKYTOI/gNCdYOk2nXpP7aW7T2V/NbXYUOOkvRrAPirR2UQ9Zxh0apgHMDpHLHOykwLs7GxUwfmJsaoAECAGedgCpQSkNQGpw1HsyIPH5MxMs/ebRk/wyzeEE/sLmBvnsRAAyIEfbgKlB84s2aGrgTAWa+vpO4MwEmkd1QERHjXDMJL/UHXYgvJOA5nPRUcAAZtFNWeF0+In1cYu7HJsEioCkQ5zWnKLeazB07gEu1TDQ2E2L0XrP9x8Z65P+yy2/zNmHmuSUqQH53MSA9J4ckGMmtwfNZOB0iujt9JrpjEI22GtuU3Oa7N1wmxh8yNqez3R1hbM6SrMnBjBLc17zMqobV8SU/hL3Ha1r1KCo905QdH7OBo6ApOYI9f7zE3Az8sl3Zoel2tcqv5kbVuDaZ4oFmAnQTALObCK61m3q/um+a9DXnHIycLaBZntnd63bBDB/Uh2SSBpDVV4mydzEUwoYwPSUnD3rYpdTf3yML2Ft1kCjgce5T3CQtnituUbnXgOj3gOQqtnGG0Trz04KyPGT/XLB5ANy3LSQ9JkckGOnBNQewNSg6eQ2AcxM4rnuAxyss896PMkieoLTdIjnqRzdAcxrlkaktWcti5IerQHMkp7e3A2utVXkvxZYgJkAwzSiNICkQdC5d4Ih4rI21Dj3AF27VhmTo59ZKUf3myrHTQ3K8VODctyUgGoA57E2aKYCTiWi29fT99BWdPrlBE7GpMEzAZxJIjocpwbPZG4TLhN3q2e6uU5R7lZAz7yyAcyeOe9dHnWsuVV+h5N4uS8BNGmBMcnNJxU4JkBxkl9ZueEKAbh2rSIkR1cEpc//jJE+P39NThizX06oDMvxFQE53gZNwFNznHCfmYDTCZodAk5UEEmeAMnACV0+Pd4nE/Ye6QbVZeKbC3QbBQxgdhvpi/vGwaZW+fWcsAqHbMc1dgQc4fZsN6B2wDjJMt5g9U40jDqz4tJn0Grpc/EQ6XPJUDlx+CY5qSomJwCYqUDTIaojotP0ffR9M4GmNj4lc5pwnNryr/WaydwmFvIzppF0w1jIi3ult++9Acz29DD/uaSABsxPjPMp5/BUXCNgpDjHTFxjEigebQObBjj2Sjc5NSR9KkJy7P+Nlz4/tgDzhIcWyslVcTmxIqBaAjgrLBFdc5u9k41C9j0BT4BT95Mx6KZfAnDNSrdp6zcT4rntFpWs39TACWBeWhNKG5nkkszmsAKjgAHMApuQYukOgPnbuWFltU6I0x3gGJ2AmABFDY6TLSMOIjUWcGXYmRWT44Zvlj4/e1W13hcNluMHVsopVXE5eVpQTpqaBJrJIroNmvq+mnN1cpyZuE0FnCn0morbTBLREc97jfHLbatM4bNiWc9u+2kA0y2lzHHtKEDhM2K3cfPR4q0GH70HlI5xcJAarPRecY42SCpw1EBp6yE1h3hsRVCOmxmVYwdWSp8fD5Y+v3xdel80RI67cpKcUhmWk6eH5OSpNmhODSgRXXObToNQsk6Tfui+slfj8CBCCEd+jEAf7jL6y3aLpgT+MYBZApPYHUPAVeYvi6Iq/Rpgozk2BZLpxGoNjjbXCPeoLNuOvQY4tbf1ksdXhuWEcYfk2D+8L31+Mkz6/OJ16X3JUDnuL+Vy6mSfAs1TpgaFdpIGTltMR7fpvKY2CCWMQY6+O0ETrhmO040xCB1nQlQf51fO/GdMC8rqgIt8dN0xeeaenaaAAcxOky5/JxIp8uHORnl9W4MM3mI1ytC+vb1BqK2NJXb2wSZZ5GuWDaEWOdyQH8+/vstjKvQxFccIIKXiGjVAOkGMz1i5VbMNOHCIup1YFZMTX1krx146TPr87DUFmADncf81Uk4bf0hOmxmRU6cF5RTa1GAbt+m4hrai6/trbjMBnCmc3Z2gmQycyiCUQkRHp4lvKgYxrwq45W+lmTtlo4ABzGwUKoDfp+1vVA9hr5EkpE1qoyhLa1Ut/Mw4v5w4OSDfnhlUqdD+uigi96yJyXs7G2V+fbPsiLZIi4dY+sDauDKIJMARztHBLQJOycCYAMdUwGhzhSeij5waUHpJOEYMO8ffWiV9LhpigeUvXpc+Px0mx/1phJw+7qCcPisip00LKtDUwImInhDTbeB0cpsaOOm7bgC/5pSd3KZTt+k0BmmDkDIK2eCJ9ZxCa09uiBfAyjFd8JoCBjC9pmgOrlff2CrnzwxKr5H1FnBS6zpVs8GTCJNeZfXW8XxW0Tg++cr0oPzb3LAyRpTvaVR1ZprwQO/k9v7OBsVFAjQaHPVec4waILVOUXONeq8t3BhtnA2wU9zijLCKFz/ub2OlN+5EgCUiuQ2YXx53UM6YHZHTpwfl9GnBBHBqET0VaDq5TW1USuY0Ac5k0ExlQU+Apg2Y/zTWrxJuwPGbrfQoYACzSOb0lpVRCwTH+CzQdOydlQp1QS5nvRmcqFVuRgfI4v5y+tSA/Gl+WAZtapCl/maJd5D9XOxvljOnBwRXIOUHmYVrTAZEzQVqcEOkhkN0ttNmx+TU8v1y7L++LX0uHd4GmD8ZqkTyMyYckjNnReSM6UH5MqA53QJNzXG2E9GTDEJO4ATok8V0DZrZgBOuU4noJAcp98lPakLib+z8i6hIlmSP7KYBzCKZdjgW6lsjfsMxOpsGTA2W7AFMVZDMLh0B53OUowQECXk/NtbSt1Eo7OSKgLJ6v7atQXbG3Bkr9sdb5bK6kHxpkr9N36jF6oojOUYNkJp7BMzagaPNIQJ2NDjGM2ricurbW6U3YIlLkeYwLxkqJ/ytXM6a6pezZoXlzBnBBGgCnPoaThHdaRDSHK9WGWgR3S1oaoMQ4Z0AJpwmxh9o+fA6405UJI9Vh7tpALPDJOueE6LNrXJZbahNLLdBU4Mlew2YmrsknlmXVVCA6chPiVWXB1xzRhgrOP4zEwLyvdkheWx9PGs9cHiogSuj8sVJ/oQ47QTFdMAIiGlA08CoRGqbSwTwaHCNZ9bE5bTnVkpvrOM/tw0+iOQXDZETr5ks51RF5OyZITlrRlCBphM4NbepQbkdt+nQa2pOE/BUnKaD20zWa2pu06nX1AYhuPYTpwRkXr0Rx7vnKcn9XQ1g5p7Gnt1h6Ja4Kjim9JdZANPJXQKaOtaZhxrDhAZMOCM4JB56yyockE9PCCSA86Ut8YwVD0fsbpQTK4KqadFaA5TeuwVHBZAO4AP8zqqNyakPzZc+PxkqfX5u6S/hMo+5aLCccluVnDunQc6pDMrZHOs4V4NuMmhqQFeO7knO7sncptMYBHAClrppC7oTOHnh/GFeWOImnZtna77QLmQAs9BmJEN/9sZa5ZzpgYQuM8FdlrfnLp1gCWdJAygVWNo+g3CXyWAJGAAMCiimBOTzEy2A+NPCsKzwp46J3hFtlYtqw3Jcha0/tPWI6TjGMxCdHcCmQNEGu7MAPkcDCL9SF5dT76ppZ/CB0zzmkqHy5WeWy9fmNMhXZ4bkK5UhCzgr2wNnsm4TED/C/SgJODXHmSymQxvNcSaAk9DKSQFLJJ/glze2N2SYQfNTsVPAAGaRzeD9a2IJsVwD5hGiuF0jXFdxhLtM5iwRxTVgwiUR5aKdztHjaev2cRUWIJw/OyQT9qUWNe9dGxeO01wdIKUbgKgbHKBucIQ0QFG3rwCQlRb4sf/qrJB8bU5cTrl1thAKmdBfXvaqEs+/8sEO+UZdXM6dGUqAJtdI5jiTQROOVwGnw/VIcZxOMd12eE8loqcCTZIof392SPbEUFSYrVQpYACzyGZ2U7hF+VriNqQAM4m7VMaeFIYelV3H5i7J9dgOLCf6lahJGCPcJSCB1RvDCIYSRO0+U4JyzoygvLXjSA5qoa9ZvjkrpEBIi8UaGBXXCDg6gFFxjoCjzRnCIeoG+J07K6RA8GuzQvL1uSkAE4PP/4ySb0z3yzeqo/L1mSH5Guc5gFOBps1t6j4BnJrzTYBmsrN7kiXdKabzItFGIQ2avGR42aADfsLkviyyp6nj3TWA2XGadfsZd69u4zJTcZfa0NNOb+kQxeEutd4S7hLxkgcfsMQvEZAALPGR1JZsLNYnTrW4QhzhnRsquxtXxZSzeTLXaIFjSIGjBkX2ChhtoAPsFDgCkLNC8g3dZofkvLlxOe02OMw2H8xjLnxFTr+nRr49r0HOm20dz3kaNLm25lS5vwZvON1kblPrWRHT2+k37RcGdHCCpuI4bbWFFtFRb3x7Zki2Rw136VwXpfjZAGYRzioRO6dUBJSLkRMwk3WXWhRHHNdGnoQobsdJY7zQuku4J81dApaIqadOtSzaiNsADgADNznVUbkREi4LNMv3qkJy+vQ2btEJjIBYMjA6wfG8WSEFft+cHRLdvlUVku/Mi8sZd9fKMbbTeu/LXhXa197ZIufPb5Bv2cdrkFXAaXOo3D+h23QYhZQeNclvMwGcCTEdQ1b7DEhO4OTFwguGF82nJ/jluU0msqcIH6UOd9kAZodJVhgnPL4+row/OEqncyNKGHpSACZWcafuUnGXWncJUEwNKI4LIEGMhTODU4NjO216SC6bE5aN4fb+ms9ubpDTZwSVeJ0MjhrQ9B7OkJYAx9khBX7frgqJbt+pCsl358fl7EfnKyOPso5fPEROvny8nF8TlvNrI+pYDZqpuM12oOnCIMR427kfOUV0R6in5jQ/NcEvl9SE5FCe4vcLY/X13F4YwCzSuT/c2CrfqAwq0EzppJ7C0AMnhO4SZ2snWBKpA2AqDsrmqpQPpXYet7lL9JDoHdExfnlGSAauikqDIzrI1yjy34ujCmABRgVgAOOsNmAEIAE4uMdvz7YawKjb+dVhOb/Kat+tDsv3FzTKV19cKcf8ZJj0vvRVOebioXLu4DXyg0VNcr59HgDL9biuviecJrpN+gpoJoATTrOyzRAFt6mNVckuSAngdBqDHKB59JSg4sgnpzGGFenSMt3OQAEDmBmIU+g/kanoY6OtxBvaSR3L+FF2RI/2t0QcT4jits9lsu4SjknrLhHFAQsMI1oU19wlorUWewGnifvbW86XB5qVm9HZlTYwanC0OccEMFaFRIFjdVgARtr37Pb9mrCoVh2WHy1slG9+sF0B5tE/ekVO+lu5/KAmLD+YE1XHc1474LS51nbcZioRPYtuk7Fr96OTSFA8IywnzozICbNjckJNgxxX3SBfrAip5CaFvk5M/7yjgAFM72iZ9yvB3P1xXli5GbUBplXJ0I0bkdZdwl0q3SXGHlsUBywADS2KY8z5aqVlrIF7hFP86syg/H1ZVELtMVMm7WtUIIYhpx1AApKI2ckAWR1WIAhI/hAwtPd8vnBRg3xv8gHlUnT0ha/It97cJBcubkwcD8i6As0MVnTt9pQwCDF2witnRuS0mricUh2XU6aF5KRR++SEl9fJ8Q8vlN79psrJD8yTy5fHpf+KmFy5JCJ9l0XkxpVRuX9tTJ7dFJc3tjXIpL2NKk7/QEOrSfeW9yfE+xsawPSepnm94opAs6rTTUlX7aSOscfJXaZzUk9lGdfcJbo8zV0Clvg3au4S7k2J01Uh+V5NWCqSuEwI8N6uRvlOVVi+ObuNcwTcNPeouEQHMP6oJiwX1LZvF9aF5aL5MbmgKiDH/ds7cnq/Crl4UYNcOCciP6ptA1euqUE4FbcJwGuumDFo8ZwxKUd5h0HozMqQnFEdkzNq43LaZJ+cPHi9Si133H+Pkj6/etNKAPLdQdLnx69I79e2yKenRlQSZTwTeGl9bKyd6ETpli0VyEkVAfn6jJBcVhuWvsui8uLmBpm8r1G2RJoNiOb1aen6zQxgdp2G3X4FLLS9xtTLx8rTc5dON6JkJ3XFXTp0l6kMPXCXGHIAH3SFcI6A1Ddmh+WOtXFpSuFR8+6uRgWQ36xqzzUCdjQNkBfWhkW1urD82G4XAZZ1Ybl4bkQumhuVM++rkx+U75FLFjfIj+1zk0EzwW1Wh1X/lG4zlYjejtu0Hd0Bz5qYnFUbly+P2CUn3Vkjx/7Hhyoks/eFrwhVKvvgMH/JELU/dtgm6V3VqKzkcOrQFDUHxjT0xKhAUIXw8iIhh0qWQrYoQlpH+9QL7cxpAflZbUhuXxWTsl2Nsj7ULCaqstsfp4wdMICZkTzF8WO0WVQMM5mMtCuRGzciLYq3013aoniCu7Sjb+DMtCgOEH23KqTAED3kvy6IyNY0Pohj9zXJz+ZG5FtVFkimBcY5Ybl4TlguUS0il8yJyE/mROSiORH51fyo9F0ekV8sjKn/AVKAlWsBuumAU6sD4IZRIbTTazpA86vVUTl3bqOcOWqPnHTjTOnzqzek948HWwk/dIb3X1pJi1Xi4mdXyrFVDSpMEl/MZMDk5aRBE+6euYDjR03C/KhMUeV+K6cp+UpH1avEKadP9ctv54TkifVxqTnUZFLEFeDjZwCzACelM11aFWiWM6cFE9xLMmBmdCOaYvkbYhlPyV3aPpRYuzV3CTeHjlHpGevCMnF/6lhzxjLf1yz/uySqXIEAuIvhHOs0OLK3wBGA/Onctsb/F9RFZPC2RlkRbpV/XRiVH9dZYHrRHBs0beBMB5pOEf0I0MRBfm6jnFsZklPvnyfH/subAjepUsnZaeR0OKaqJfSzV+X451bIcVVx6VMRVJ4FOuIHDhOfVuiMF8IRgJkEmkfZMf5alE/kKyXx8yifOv9Hs4Ny26qoTD/QKD6TX7Mzj4Xn5xjA9Jyk3XdBqhQiCn5sbFvqNrgdLY7zQKO35CFPOKnbUT2EQCondduhG0OIMvQ4onAAS8VdoovEgo0oXWfpJV/Y2j76J5kKe+Kt8sjGuALEH9ZaoAc4Xmq3y+ZFJLkBloj7cNAkN+6/KqoA9CdzrfPhSBPAaXObGsS1rlQZhJJEdDjN86rD8s0FTfLVEbvlhL+UC9FDZHFPAKQTMC8aLMf++i054ZW1ckJVTKhiqR3XNWAqLtNROC0rYMJpjvUn0u+h/yTwABcxQl5VRioFnvVy1FifXFAVUsakhfXNYrAzeXXl738DmPmjdV7udO/amHx0rE+Jfzy0CbC0U5NpQ08qN6J2hp4kUVwZemwrN2CEkQaRGPH4R3VhuWVtXLLlHUY/N+1gs/xjRUx+PCei2qXzIvKzpAZw/qguItetiglJivU2ZHuDXFhngSxgCwcKd6qAU4vpdW1iOoYlBZzJlvSaiJy/sFHOGbxGjv2Xt+QYSvc6AVJ/Rmd54Sty3H+VyUnvbJcTq+Ny/NSgcr+Cfqg0FGAmxPL2lSahPyJ5KrFcieZpABPQ1BFcvcpt8KQ8SZlPjp7ol9/NDctb2xuE7FVmyy8FDGDml945v1uoqVX+uigqHyn3C0k2MEQ4fS41d4nekgQbhP8lRPFplkO39rnEmox1GbBMFsURrQFLAAsx+a/Loq6jXQ41tMrbu5rkf5dG5WJbTwmnCXBqsOy7Mia7khB4kb9Zfr0gInCYHMc5TuCkPzS43lS6TWVJr4vK9xY2ytlPLZbePx1u6Sk1QDr2vS9GhzlUTrxpppwy4bAqxEZcuY4tBzCdiTjS6TGzASZ+s1os1xymEzABTp2VShmMyLgPeI72yXkzgvLA2pisDbaPuMr5IuvBNzCAWYKTvy/eKr+dG1ag+Tk7MTAPNGCZ0tCT5KSuInqSfC4TojjcZW1YWaox0ABegN6fFkc7nHxid6xVsKRfsTymrpUh1p4AACAASURBVIMI/sO6iPRfFRN+S96CTYjlFneqwVWD5k8Vt2kZjgDNdkYh27fzB3VRFSF09hOLVV1zADOZsySze+8LB8vx/zlSTn15jZxeFZNTZ0blZEfOTBUR5RIwUZEkOMzxbXlJNYfZTix3hLlqDtMJmO2AE4s7InuZT06eEpCBK6KyPE3O0mQ6mv87TwEDmJ2nXUGfuSXcIpfWhpQ+kwS3GizbZSNKihc/k3hxlYbN8lXU3CVgifEEvSU6Qm24QSQGsADN3y2MyOpgesNPJmLBcU7a3yQ3ronLHevisj9DXPb7uxuVqxEcphblndzmJU79ppPbrIvIhUua5dznV1hgSY2gX74hfbB+//w19R16zON+/67K8H7m5Ho5s7ZBTq+kaqWVyQhuHA6TBMPJHCaqDqceE1VIKj2mju8HMI9yGH4Ul+kATCeXqYFS7531nCyu06oQ2nuiX+5dEzMZ3zMtti7+ZgCziwQs5NM3R1rkF3Vh+cQ4KyPRkYaegAr/c4ZAklwDUZwoHe1GpH0ule4S7tK2cCMOA1YA5m8XRASRuSsbxh1apm1btFX+c4klymvATOY26Y9Tt4mIfvGyJvnWO1uk989es8RwBZLD5JgLXpHeFw+VE/5zpJz+8AI5e+IhOWdOg5xVZVWiJL4c3S5qC5z6FWDaKd8yGX5SAWaye5HmMgFLLZYDlLppLlMDpd4fAZgYiWxR/a7V0ZQ+sZloan5zTwEDmO5pVZRH7o61yH8tjMgnx1vO1U6fy4TukmxEdrQL6dCSI3rwtdRuRBh6MLIo7tLm8gCo3yyIyDxfFrTzgIII6s9usYw/Ti5Tg6bmNjVoKj3rkgb54eT9cuyv3pAvfHuQHH3hYJXE4/jfvyenDpwh5wxdJ1+bEZCvzWsUfDLxDkCPS6ikBky4TAATLhPdL3QEMHkJwb0nOMxJls44G2AmfDJdGH5wOdJgqfftQFOJ5z65Zmm0w6WSPZiSHnUJA5g9YLoxBN2zNq4ecHSacEmAZXK8eDvu0jb0IIoDllicEcUt7tIytmjAApwwxuBvmY9tVahF3e/7uBJh4ElqWO1/qBzaI3LBvJhcUheWL187SYnfp1w5Qc66b458/Y1N8q1p9fLt+Q3yzXkN8nWc1+38mU7AxIEfDhNaJQOmUyxPBkz8MQFNdJgY3xJ6TNuB3TPABCxH1Muf50eEeTZbbilgADO39C2oq0/Z3yg/rQ3JFyb5pfcUCwQABLhLy9BjxYtrUdxp6AGAlGg7J6ws05fZ4jhiOYaaX82PyIpO6jA7SiRswhP2N8nTmxvkkQ1xuXd9XO5aZ+k/714fl/vXx+WJzXEZur1Rxh1olntXReT88t3yg0qfXLAgLj9c3CzfW9Ao58+JqggkvADQ12rA1Fna0elCH525iJcMeULbWcqd7kVJIZIJLjMLYGpLebIeU4vkGQ0/I+vl93PDUm+cMzu6jDp1vAHMTpGteE+qb2iV5zfH5cc1YSVaIl7CQWHs0bpL5UZk55rUoriyjKMLVH6Plg8l3OYv50dkwKqYKpBG7fRC2/yNrXL5irj8cEFcLpwfE5zm0cXiYoRuFncpJ2ASM58ATDtXJoCp9ZgaMBOGHzeA6bSUp+AwO2Uph7McWa+yVR3OYCQrtPko9v4YwCz2Gexk/6lu+OaOBvmfxVFlAYeTQl93piNJMIk1iAH/jk7HhktRXUR+Mz8iVy6PyUvbGmShv1niBewGSFjl92vwFbVUClj52wFmVVuMOclFeGkAmIjlRDtpDlOJ5U7Dj20pb+fA7rCUa5H8CEt5EmAe5bCUO6N9nFbyZA5TGXjK6uX/FkdMvHkn139nTzOA2VnKlch5gN3yQLO8u7NRHlgflyuXR+WPCyPyuwUR+f3CiPzHoqj8ZWlUBq6OCSUoyvY0ytJAswSKQF+2JdIif1hkhVMqh3b0mjZgwjmjn9WJOVBD4BlgieWWiiJh+HFwmMmWcq3HbGf4sWPKEyK5k8N0xJSn02MmW8mdgKnAcrRPblgRlVh+VMYlstK9GYYBTG/oWDJXAUABQ0pgHGxoVboxHMYbCpiLTEf8l7Y1Kv0qFn2SfaCDBTAxYCnArA6nAcw2sTybpTwdYBJdhYHNyWFi+KEcr85clA4wU0X8qOQcZfVyVLlfntwQl8JTfqSbhdL63gBmac2nGY1NgZ2xVvnz4qhcVGf5ieIKpcXydoCZQo+pK00mOEzbUk5yEmX4SQ6RnJI9pjybpVwbflKJ5RZY+uTYSX55Z0fmJCdmAeSWAgYwc0tfc/VuosAHuxut5B52vLkTMJUeE5HcpeHHWRwtGTC1P2YiEYdLSzlcpor2sbMWacNPMmAqf8uyevnOzKDKkdlN5DS3tSlgANMshZKjACqEq+2MSPiK4vpE5I/WY6Yz/KgyFokSFqkNPwBmQo9pF45LZfjR2dez6TGTQySdgKlSvI32yX8tCMv2SBHqREpuZYkYwCzBSe3pQ0L/evnyqLLo68gfJ2A6DT/fqbZyfJJcWBl+EoAZamcpT4740THlcJheAia+mBh9epXVyxcm+OWJDXGJFaC7Vk9dYwYwe+rMl/i4seSTQUnlzyTe3eYwqQeET2lCj2mX/9WA6XRg1xE/2uVKObDrEMmpbSGS2vCDWJ6I+MmQTFjFlKfIXARY6mQa358VlGn7jb6y0JapAcxCm5Ec9AeH8p2xFlkfapG1oRbZFG6Rg47EvDm4ZUFccr6/Wf642M7SbicadgIm/pi6hAUO7N9wRPwoB/Y0MeXJesx2gKmTCTsA0xkiCVimAsxPUChtVL388zifXLc8KnuScoEWBEFNJ4xIXqprYEe0RUbsalB1sv+DpLu1Yfnu7JBqF9WG5Q8LIjJgZVRe3dEga0ItUqoasgX+Zvnzkqh8v9bKzK4t5VqP6QTMdhE/tgO701LeLkTSkepNGX7sXKP4YzrTvKV1YHdwmCR7xr/y2zODUr7HcJWF/EwaDrOQZ6cTfas73CQDV0blmzND6sHFD/CLk/zSZ0pQTqgIqtyOpxEjPSMkX54RlK/OCqnkFLevjauonU7csuBPWRlsVs7336WshouIH5zXdcSPBkxnxE87w48jc1E7S3kSh+n0x9RcpuIqR/vkCxMDctuqmJICCp6YPbyDBjBLZAHMq2+Svy2KqIzqWFo/NcHidBAXdXYiwvt48AEBRE70dbqS4tdnkbItLM9taVBO6yVClsQwtkdb5YbVMRXmiSiuOUwdU06iEThMHfEDYH4lTYhkKsBMNvw465QnW8pxWEdXSR3538wJG3ehxCwV/gcDmIU/Rxl7uC3aosLkvjTRegh5GHUNH8RDHmQSRaiqkA7AxDmb2Gl0d4ilGEHIqP7dmohctSKmdJ0Zb1yEP4abWuXFrQ3Kvehbs62Y8kSIZJX18tCWcmeIZMaYcpvD1HpMZ4hksmsRnKXKbVnulx9WheT9nY1FGUFVhFPvWZcNYHpGyvxf6J3tDXK2XYtcl9ZF9ENvBofDw0uSWwCTLDskwcU9hnRuJJgAMOEwydoD16UKm82JKH0fUTKL/aWp2aw+3KTqpJ83OyznkVykKqzKB6e0lFdaSThUiKQdU+5MJtwu1VtyMmFbLP/0BL+q5PnxsX75zqygvLKlQfxFEIuf/xVd+Hc0gFn4c3RED7dFWuTviyPyUbt+Nc7PRI4Qq5wATGqQ2/XHMUoAmIiSGC7I86gB87xZlh8inBb+icRc4+hNQTIszGQjKsUNX81h2xvl1/Mjcu6ssEq8AS1o2rXImepNx5SnMvwcV2HX+LFTveFahF4SoMRVCJH8pzUhBZTOssGlSNdSH5MBzCKb4Wn7m+TrMwLKsRnxjpA6xHAAE5GPDN88oF+0ARNDhAJMO8s6Dzx6THwMAQREUMRydHno9bAik0Gdkg8kBr5qRVQiJew4jUrjxS0N8tv5EQWapLc7uxL9pdV0bky4ch0iSfb1dq5FFRYnzwvqcxMD8s9kJ5oQkDOnB+QviyIyenejBEyC3yJ70lJ31wBmaroU3Ldg1nOb4ir6gygQokHgXgBMOExA0wmYWo+pAZNEwYT0acNPNsAkQgYu84lNcSlhvEzMMzXQy3Y3yYCVMblsTlileQMkT5lm0e2kaWRbD8rxFUE5riKovA6OnuxX2es/P8lSgUDrr1eGVA2lFzc3yLIS5c4TROuBHwxgFsGkq3rcy6LyEbJsj7ZC50gB5hYwj51sRaU4ARNLOVZgckCiu8Pwgx6TImeUqiWzOnV6cMnpSRtaW6ptTtnfJIM2N0i/5VH508KI/GpuWC6hTEd1WC6oDstFNWH5eV1Ifjc/LFctjcrjG+Iydk+jrAm2mKqNJbxgDGAW+OTui7fKn+aHlQiOc7Ou8wKHifsQoOnkMJ16TByoleHHBkwqHmrDTwIwnYYfwMAuQ/GD2og8vsnkXWxpFaWS2Bdvkc3hFtkQsiKmtkZaVL5QoqhaC3wNme55RwEDmN7R0vMr7Yq2yL/UhVXtFjhLVWK13AJNJ2BqsZzktE7AxFKOAYJSsOgxtT+mNvwgluM+gx4TP0QMP1jK4TTJVL7JZMjxfE7NBYubAgYwC3T+4GB+mQyWY3zKjw8uMx1gaj2mdi0CMLUeUwMmekysvk7A/FaVZfj5Hk7dtWEZtaepQCnT9W7BEZJNfnWoWaYeaFLlOYZsa5CXtzbKy1sbZPj2Bhm9p1EW+JqF2kc9QYfbdar2jCsYwCzQeX5wbUzVm6YyoMqLCFiOSRLJO6LHxB8zheEHazBuNFjKVbRLVVie3BQvST3c1miLjNnbJA9uiCs/zF/Nt7wCSCRMwbdzZ4XkKzODqsF1f786JL+YE5brVkTlnZ0NgjuX2Xo2BQxgFuj874y2KKv4tyuDKjGDAs5RPunlFMnTAeZ4y7VIW8qPQSxPRPy0Wcq1HvPrM62oH8Tzu9fFJVRCTtXRZpGaw83y0Ia4Svd20ZyI/KCGZqV4I7oJrhq3Kl4YOmsRtMC96MvTQ8q7gPj7H9eE5L61cVkR6FmGsAJ9RLqlWwYwu4Xs7m+K6PjG9galy/w0Mchl9VYbbXGbGH6oB3OU7Y+JSI4eE19MJ2ASuqf9MbXhh5C/syvxL7QqJj6+MS6hEpI/F/qa5frVMfn5PMrsWpZ/EgkfUa4iGTAdzutw4LxY8F3FrYgkJiQ2eWR9XA70gBR57ldqzzjSAGaRzHNji8jcw81yz5qYXFgVlE+N96n8ieRQ7DUav0zbHzOd4ccGTEuPaVnLEdFPnhaUf5kbkbF7SyutGHrKRzY2CBmKiFxKm3ndrrlOeCh63HYp3iqt6pHOWHKc1o+dEpQvTArIT2tDMuNA6ep6i+TRyGs3DWDmldze3IzImwX1TfLcxrj8cX5YvjYjKJ+yM+CojN22cegT43zyyXE++dT4gAqZ/Kxd+vVLk/3Kgf238yIyZGuDlGK4HoD5+KYGFblE1BKASQQToZ9EM2XKuk6Mfar0bhjL4M6J8uHFQxndkysC8vKWuDcTa65S8BQwgFnwU5S9g/hq1h5qkre3N8idq2OqaNaltSH5UVVQvjuLFpIfVYfkZ3Uh+cviiDy9KS5Vh5olWOLM0bNbGlTlSMBSF0LLlEDYmXEdURwvgmTuUoVETrVctEhqAqf52YkBuWtNTJACzFbaFDCAWaLziyoy3NwqB+ItsjfeonJcNvSwB3rINrvUrs1dXkRtcrumD76mOhcmorjOVHQEd2nHkGvukjR5BACgzkAnjG4Yf9d/Hh+QgStjpmBZiT5PelgGMDUlzL7kKPD+rkZVBA1R/BLA0s62niiAhmV8tgMsZ7WJ4oq7JDP9dCvDk5VwI9AOLPE8ICiAaKrPTwzIUeP9cvPKqDS2ohAwWylSwABmKc6qGZOiwKT9TYIbEXHxuia5LrGb4C5ntzf0KFG80spKr1O6wV0iijvrkeuEwQQFkKWIMFRS631yvE8eWhczM1CiFDCAWaITa4YlssjfLL+aH1F1fI7gLu3yusllKXQ6t3a6SwWWR4rigKXOsI5YTko9wlNx6Xp1W4OZghKkgAHMEpxUMySLAhjD/t+SqPyALEO1bXV8iOxRbkSOGj44qjsNPc4M607uUusttSieqg75J8b65aQpAZl50Di4l9paNIBZajNqxpOgAIavW9bEBIDUbkTpRHHciHDix0ldi+I6u7oWxQFLGmCpuMtJdkldlWHdKg0Cd4lo3muMX3knkBPAbKVDAQOYpTOXZiQpKPDurkYFmFjF0xU8c/pcOkVxrbtMtoo7RXF0l8nFzsh6j2iOT+z/LY5Ig7EBpZiZ4vzKAGZxzpvptUsKrA62yM/nhuVbVWH5rip41mYVJ+kIZTpUOV272BlWcUpRtLkRWT6XyaI4QKn1lgAmnCUNsCQ0lRBVRHMSPb+8xegzXU5XwR9mALPgp8h0sCsUaGqxxPKvzWrTW2pDD3pLADOTKK64S9vfUoviqfSWWhT/jB2aCodJnSW4zJMrgrLUZ/SZXZnHQjnXAGahzITpR84oUHmoSYnjRPKkc1BPKYrbET1Kb2lXhMQqDneZThTX3CVgSZ0lsuGTKf/3c8MSJ3272YqaAgYwi3r6TOfdUACgun5VTM6qDKrs8rqM7hFWce2kruLF25zUtc+ldiFS4rhdc5xEzVoU19wl4jhgqQGTxCiURB621cScu5mvQj7GAGYhz04B943CbPsbWmVXrFV2xFokVuDG4MX+ZlV3nRyXKvzRzkSEVZw67c6IHh3+SIINt1ZxOMtU3OU/jfWr9HtklPrytIBJQlzAa9pN1wxguqGSOUZRYGOkRd7b1Sj3ro/LFctjKiHv7xdF5dcLovLmrtynh/M1tnYJcChDQencc7ShxwGW2tDj9LkkuQbcpdMqjqEnlVU8AZY2d4koTgMwKVT3sbE+lcd0wPKIWU1FTAEDmEU8efnq+sZwi9y9Nq5SoyHOftMuyUuFSUIPf1gXkb8ti6o6ObnqE4mUb1wdk98siMh96+Mqi3pHE8PHmlvlhpUxocY4OssjfC7t1G2p3Ih0+KMTMPG3pDkt407dpQZLAJNEz5QaIQk0eU3NVpwUMIBZnPOWt16P29ckF9WGlZsNVmVdXfKHdg1zYrSJ1SaFWmWOIlsQ/29fG5cf1kbk4rkR+VFdWC6dF5Fb18YFUbsjG8D7v4sjKtsQYrhTFHdyl8luRFjGnWCZsIo73IiwjCd0l0oUt7hLBZh24Toy5v9hXtgUVuvIpBXQsQYwC2gyCq0rb+1oVOIrmXoAS8Vd2sXSzre5TCpMEqf9vdqwvLDVe7Ec1eigrQ1yQZ0FyiQDppGBCACl/MTgbQ0qlZ1b+u2OtSjQPLYiKIzN6XOp3YicustkN6J0YJmJu8Qfk0qfuBnxedI+72nldvzmuM5TwABm52lX0mdSsgLu64SKNvGVxBQAJxUVcc8hHptQQ7jN71SH5aY1Mc+NP9MONslP50XkkrkRuXSuBZQ/mUO6Nqs2D3Hi364Oy8DVMWWEcjspcJo3rIqp8VFyAu4SsNSGHqfuUjupO3WXWgxHd6l9LrVV/Cjb0KNFcQ2WACYlkuEyf1kXkp6Wn9Tt3BTycQYwC3l2uqlvG0It8t3ZQfnS5IDiwIipBjzR+2FlJpQQSzMO4FRZJFb7vNlh+cuSqBxq9M7XUInPS6Py/VoLMC+qi8iP54RV9qELSARsl5og7PHc2WG5ekVMDncgDpGuUqKDqpmfn2QZeJyJgTH2JHSXE61YcdyIjtBbOkVxh6EnLWCO8cknyn0yocTqKHXTcs3rbQ1g5pXchX8z4O6mlVH59AR/osok3BdiK5ZkrMwYSzS3iZhOxnJqev/Hwqjs6wBgZaPGmzsaFecKOCqAJONQrV0e144NJz5cJ9Q4qzIkt3eiVARp4P6KXnOKldMSjlJZxnVyDe1zaRt5AExtFU/2ucTQQxXPdGAJh/kRuMyR9cqZvYSKdGabzpL43QBmSUyjd4NY6m9WfolkEEcsBUQQU/FNpABYgtucbnGbX4HbnBWSs2eG5M+LIp5Zyv1NrfLfi6PyzaqwwEGq+uE2OJ5PTLhdR1zVErdLTNCPc2aG5IPdHdcPIh6jhvjPhRE5qYKSE8SDW5nU4Sq1g3oq7hLdpdONCMDEKk5DDNeiOGCpAHMMFT998plxfqk6WOKFlbxbmgVxJQOYBTENhdOJB9fF5KhxPvnixIBg7IDTAjgxgljAaZXoTQCnzW2eNj0k1y6PSdwjB/ZZh5qVRR6RH10pjXIS/I/+VMeDo08FKFERoF+FC/7ZnLDs6WTNcPo/82CT3LQyJt+ZFVI+lwAfiTTQUeq67+y1kUfrLtu5EdlAmQosPwJg0kbWy9VLjV9m4az+7D0xgJmdRj3mCBzDL6kJKs4Ijgo3Gi2ekngC0ETHd+LUgDKSJMT06ZRvCMrjG73LyjNoS4MKZQQcFUDOsoxNqAAARw2Q6FMJcaTpbOknTwso3WRXJ25HtEVG7GqQG1ZE5YKqkOK2AU4Fdkq09qt68HxH04CpuMssgKlAs6xeTpnily3hlq521ZyfJwoYwMwToYvhNrMPNknvSQHFSSF6fn6CX3FYgKbmNjVwOrlNwPKcGUGZ5ZF4SY6Kq5dHVSJfDDIJcMTgVBlU6dgAR7IMUawMnepZGKXQsU4PKlD//fyIhDrq2Z5hkg41tErd4SZ5aXNc+i2PyqU1ITmlIqBo9HFAdBRidn37NtpyVsdhXTV1jCWOq+PLfNJrRL08u9HEmGcgfUH9ZACzoKajezvz/Ka4fLTcEj1JJAFofs42eFCvBuAkAQWgqfWbuOJgTf/zgkiHfCEzjZS49H+bH5FTpwOOcI4W96jTsGGtp2GAAiCx4COKoyagnVhhnbcghynV/I2tQgRU1aEmVb/n3jUxuXZZRP5jflguqQ7JeZUBOa0iIL0n+eVL6EDtF9Dnx/vlCxP8csxEv5xeEZBvVQblqQ0GMDOth0L6zQBmIc1GN/eF7OCIm05dHf6G7YDT5jYVcE6xOE9A00sXmVBzq1w2J6yqNKYCxtNtYEQlQMP5HIMU1nwa3C+gjstQvjeYWsB0b6xFKE+xPtQiJDFeFWiRlYEWWe5vVv/z/fZIi8C5ErJptuKggAHM4pinnPeS8MMLqoJKdMTiC2hi1MC4AbcJcBLhonSbNrcJx/nJ8X6l4/NQ+lWc6mV1YSVaK87R5h6TwRFVAPV20Klqp3N0rLTPTvTLbauiOaebuUHPooABzJ4132lHuzrQLF+eGlAcJj6EGDA0cAKaNHwPFbdpA+fHxvrl13PDQnVGLzdcfP5tXkSOmWJZ5DXnqMERDpKmwRFjFA01Aa3PlIAC+H8sjYi3PfNylOZaxUgBA5jFOGs56PO0/U2Kg1SxzrbjtRM0NbepHbZ7lfvk0tpQl9KtZRoGhpUvTPYrzlFzjwokbXBsB5C2XhUXKOVwPjkgn5rgl/9eGJEGk+U8E5nNbx2kgAHMDhKsVA9/Z0eD5S5jJ4cg/tniNNu4TUBTWYTL/fL7eWGVODhX9KBwGGJ1Ku4RDrL3FMsAhS6V8EXdUBPgDgV3/Kf5YaMfzNUE9dDrGsDsoROfPOwXN8Ut15gxVjSKjlCxnLYt8KTWNrrNm1dGJeCl0jK5MyIyt75ZFQ8D/Jyc4zG20SmRQQiAtC35JMfQCTI+Mc4n/70wLDnuZoqem69KmQIGMEt5djswtgfXxlQWHURyHcKnQVM5ao+ql3NnBOTDPGRWp9tYjn89J6yMShoc4R41B6lzU2qAVKGLdn0d9KwfHeuTK5aYKJoOLAFzqAsKGMB0QaSecMgtK6MJwCQKJRG+V1Yvnxvvl2uWRmRLJL8RKUO3NiiOFjDU3GM7gLQt9wAkTVnzbeMUID9wpbGS94S1m88xGsDMJ7UL+F4DV7QBpopCGVmvsu78bl5YZh7ongQRWN/PnxWUj4/zKQd6nbhXgaNdGgKQxBCljVFY80mcAZf86PpYAVPcdK0YKWAAsxhnLQd9vn55VHp9eFh6jfTJZ8dbRp3xexq7PcntC5vjytAECKYCRw2QOokvhiniuvn+/Z35d1zPwdSYSxYQBQxgFtBkdGdX7lsTk69MC8rAFTGpPtgsHuYB7tKwDje2ysXVIWXBBwQVQNqO9RigaNpflD3tI+V+wTi0MIehkV0alDm5aClgALNop87bjpOZZ2esMN28x+1tUtzlx+w496NwqrcbvqLkn9RJe3GFItHFV6cHZG+BjsfbmTNXyycFDGDmk9rmXp2iAL7nfZdFFRDqNGoAJeDobCqtGoA5ql7+vMC4FHWK2OakjBQwgJmRPObHQqHAzmiLfGdmUIGhBkaddxIne11oDGNPr7J6wU3KbIYCXlPAAKbXFDXXyxkFpu5vUu5FiNxOkNT+ouxxJyJj/JT9HS9TkbOOmwuXDAUMYJbMVPaMgTyzIS69RtcrYNQO9s494vhZ0wKeJwTpGdQ1o8xGAQOY2Shkfi8oChDqSB0cxG4dlUQVxoSj/ch6uWKxyVJUUJNWQp0xgFlCk9lThkIc+7/NDavyDojnicgkPo/yyXvG/7KnLIW8j9MAZt5Jbm7oBQX2xltVXR1q4qhYd6owltXLGVMDcsCr0pVedNRco6QoYACzpKazZw1mW7RFLq4OJjhNwPO6ZSbhRs9aBfkdrQHM/NLb3M1jCuBw/8vakPT6sF4+PsYnNYe6J+7d42GZyxUoBQxgFujEmG65pwCFxP59XljpNU09Mfd0M0d2nAIGMDtOM3NGAVIAQxDO7WYzFMglBQxg5pK65tqGAoYCJUUBA5glNZ1mMIYChgK5pIABzFxS11zbUMBQoKQoYACzpKbTDMZQwFAglxQwgJlL6pprGwoYCpQUBQxgltR0msEYChgK5JICBjBzSV1zbUMBQ4GSooABzJKaTjMYQwFDgVxSwABmLqlrrm0oYChQUhQwgFlS02kGYyhgKJBLChjAzCV1zbUNBQwFSooCY+5tjwAAIABJREFUBjBLajrNYAwFDAVySQEDmLmkrrm2oYChQElRwABmSU2nGYyhgKFALilgADOX1DXXNhQwFCgpChjALKnpNIMxFDAUyCUFDGDmkrrm2oYChgIlRQEDmCU1nWYwhgKGArmkgAHMXFLXXNtQwFCgpChgALOkptMMxlDAUCCXFDCAmUvqmmsbChgKlBQFDGCW1HSawRgKGArkkgIGMHNJXXNtQwFDgZKigAHMkppOd4NpamqSJUuWyuzZVVJdXZOyVVVVy5w58yQUCru7aAkf1dzc7IpeNbV14vf7C5ISe/fuFeY03Xzz/axZs2XDxo0F2f9C6ZQBzEKZiTz2IxQKycOPPCZ/+78r5Mqrrk3Z/n75lTLwxltk+/YdeexZYd4qFovJw488npFel19xlVzb9zpZv35DQQ6iqrpGLr/iavnHldeknG/WwV//drm88857Bdn/QumUAcxCmYmkfkQjUdm0abMsXrxE5s1fIIsWLZY1a9fJwYMHk47s+L/hcFieePIZ9eD063+9pGpXX9NPbrvtTtmxc2fHb5DmjGAwKFu2bJV169crII5Go2mOLKyvY7G4PPXksxnpdc21/eX6G26SjRs3FVbn7d7U1c1RgN6334CU880a+MdV18iHH44syP4XSqcMYBbKTNj9AFSmT6+Up59+Tm666Vbp23eAWuhwLwOuv1Huu/8hee31N2TXrt2d7jmA+eRTz8hVV/eV/tfdkLIBALfffpcngImYOm78RHn0sSfl5ltulxsG3qTA+OlnnpPKylkCB1fImwLMp57NSC/m54aBNxc0YGqwTDfnV159rQHMLAvRAGYWAuXz5/Xr19tA1k9xMzyEffu1cYAseDg/RKcZMyo73bV8AibA/vgTTyVEQcZ0bd8BAiAjBl51dT8ZOvRVCQQCnR5Prk80gJlrChfP9Q1gFshcLVu2XG697U4FLIhH6bgAvkcPBRfa2S1fgImuFE4W/V66MfESQLf29tvvCsaVQtwMYBbirHRPnwxgdg/d290VvRdgCceVCSj5DeCBy6ytndPuGh35J1+AOW36jITeL9O4rrn2OrnuuoGyatXqjgwjb8cawMwbqQv+RgYwu3mKfD6/PPTwY4przAQq+jelK7vhZtnQBeNCPgAT16Vnn3ve9bjgmkePLu/m2Uh9ewOYqenSE78tasBsbW0VHv4dO3fJ0mXLlJ9ZRcVUGTN2nHz4YZm8/8GH8sEHI2Rk2SgZO268TJ02Q6qrawXxd+fOnYLI2NLc0q3z/t77H8oV/7g6K2epARNDzUMPPypY0Tu75QMwD9fXKwMV3LDue6Y94xo67FVpaene+UhFUwOYqajSM78rOsBsbGyUzZu3KOvqa6+/qfzjbr/jLmVB5uHkwWvf+K7te+3+wTkPPvSoDBv2qkyZMlVWr1kjkS6AUGeWz6qVq+W6AQOVFTwTmOjflOvHldfI5CkVnbld4py8AObhw3Lf/Q8q9YHuf6a9Asyhww1gJmbJ2w+4FRkreddpWjSAuWfPXpk6bbo8+9wLcvPNtytQvPKqvnLVNf2UxVUvBkClrVk6v7b/rd84FuDUAMt1rh94k3J7GT1mrALkrpM28xUaGhrkuedekH+40FtqoIETfeLJpwXXo65s+QBMRHLchtyO74orr5FRRiTvyrRmPNcAZkbyuP6x4AFz27bt8t57H8htt9+V4BzR4wGCGki6uudauLrA5WB4IcJl2PDXZMeO3EW5zJ+/MOFInK3/9A+wvOvu+2Tr1m2uJzfdgfkATO49aXKF6ne2ueLl1X/AQFmzZm26Lnfr90Yk71byF9TNCxYwDxw4IB98OFJuvOlWueIf1yiOMNuDlw143P7OA/y3//uHjBo1JieTBXf57KAXslrFGS9c8D+uvFZF5vDy8GLLF2AGAkEVgolbUTraW25FV8k777xbkOI49DaA6cWqK41rFBxg4otHkoA777pHcSeK+0gTjZLuIezq9wAV3Oa4cRNyMssrVq5KqbvkvlpdoJ2677n3AcWpeZkEI1+ACfHgiO9/4CE1l4C/5YxvRS+hCmGcQ4YMl2AwlBNae3FRA5heULE0rlFQgHno0CF5/fU3E7rFrgJfZ8/XnN2kyVM8n2Us+2+9/a6Q3MKpR9WGqv79b1Ci9+DBw5RF//Dhw573IZ+ASeeJf/9wRJnce+8DSt1BiOdNN9+quE/00vF43PMxenlBA5heUrO4r1UwgInl+5FHn1B+e3BZnQE7gE5zaVYIHmF4VuOaNH7Pdm0NmBVTp3k+uxhsiH656ebbFTDCffH/62+8JZMnT1HJNg4cOJhT8TTfgKmJWF9fL2vXrlNuXTjr049i2AxgFsMs5aePBQGYiKh33nmPpdPrnzoZRDqQ0yKsMthc3VeBIkkQbrr5Nrnl1jtU4zPfAYSak9PiYSoA1YA5vQvx2ummD7eo3bt3y86du2T//gMqhprkE3Ce+dq6CzDzNT6v72MA02uKFu/1uh0wly1fIbfdTlhg+sw5yWAJoCHOcg5+jGTwGT78NaVzJBHqsmUrZMOGDbJl6zbZsnmrylHIdyR4nThpsopbfuzxp+TmW25T1wFE4UT1fTRgzpw5u3hnNkPPDWBmIE6KnwxgpiBKD/2qWwFz7br1CXchDVbZ9gAbAIeb0RtvviOLFi+RQ4cOdzhxA0aGTZu3KCfw5wa9oDhQDBDENVuA2V+qqmpKclkYwOzYtBrA7Bi9SvnobgNMxNL7738oq2uNBlBADEBDvB45sky2e+gjGYvHVKQPxhjyNVpZqfsKzr6luBnA7NisGsDsGL1K+ehuAcxYNCrPv/CS68QM6CkRv595dpCsW7c+p/OB8enV195QnGZNTV1O79VdFzeA2THKx+MN8lSRJxCeM2duVqOnSSCcfV10C2COnzBRcYtwjZqDTLe3RPB+8s677+fNV48EEPMXLMxp9mxCB0n+gcvNnj17ZNeuXbJz1y7ZvXuPYCXHmp6r/JAGMNsejEgkIgcPHlJ0h/7MA/OBmicciagDrTDPQUoVlGmddkfGdYIgfD6f7Nu7z7GGdiuDIpnuMTKyzZu3oGAAE46dPu/ft98ygCbovldI2lLIGfjzDpi4kxB66MYhHc6SWPHyMWNz6mbT9vjk7tPhw/WyZs06mTp1mjI6vfjiK/Lkk8/IAw8+LHffc7/cede9ylmf8McHHnhYHn/iaXnppcHy7rvvK0d+HMB5OLzY8gmYJDSpr/cJaezSNX7Phy8mwEcG+HnzF0rZqDEyZMgwVQrkwQcfUS5eBEswD/fce79KzPLk08/KkKHD5L33P5C7776vnWEwGTh5secDMAEaEsVMqZgqb7zxtgx6/kV59NEn5N77Hmy3hlB3Pfb4k0qSg9l4+ZXBijnJxKTkgsNkzeIRsmDBQhk9plyFHJNDgT6TnIX13kb3B5RvLpIkocnlY8fLkqXL1AvNi3XvxTXyCpgs2BdfesWVKK51lhRlKsSUX26IT9kFCpi9+ebbcv8DD6siWZZ1n0qNfZWD/tXX9FcvD77X7epr+6sXBTpbjru23wClux006EVVCpWQw65s+QTMSZOmCNFKjzzyeMpGNcb77ntIFXnrypgynbv/wAGZUTlLXnp5sDIWsraUG5oqkWHNg6a93rclZqGMRt+s/ru5BMx4LC7Ll69QL88HH3pEZeain6wP+kZfdb+de+cY6F8yyCf/7yVg7tixU5h7AJ3k2NyL/nSkzxyPFwwvAxgHfHi7e8srYGLRZkLhHJMnK/l/DC8scK+4qnwSGnFu8uQKlbfS0r9eqxYLnzO94ZNpoP/nHBY8i41FBMgsmL+w00PKJ2DC3VCDiH6najzwf7/iKpldVd3p8aQ7kQxXI8tGy5133ydXX+usk9S5edDzkWqfC8Bk7aN7JOvTddfdkJj/zq6jVP12fucFYK5du1bZAG6+lYxiXaM5656xwkCw9qnKCfOxb9/+dFOe8+/zBpjoUtxm4ObBuvPue4Xi88W0RaMxlYIOjkq/SZlw56Ls6mcWEdfmAf1wxMhOibL5BMz3Pxih4sjTjZvxoHaprqn1bKop3ztp0mQh5ykvXtYT90nXBy++9xowKdfB83JNXwssvF5HqcbcFcDcum2bDH/1dRkwYKCiOfTwmuZck6xdqLBWrFzp2XrpyIXyBphE80DAbBNPlUS40Fmzi8dpnCgdFjh6R80Fer1Ykhe4tXiukTfeeKvDXHgpAyZ1z596+lmVhxOgTKZbrv73CjAxeIwpHysDrh+oxpDtefFyPJ0BTIyT48dPVLkByCoGHbzsU6pr8YxhB0FNke8tb4D5+utvudJdorPDhQMuoVg2rNkYcRA9cw2UzgXEw0SC3o5mVSpVwMQSjJ8uXGU+54E58QIwSWmIoY/+X5MH4HGuJT53BjDRK/a1pZ7k6+Xyf0CTCEE8G/K55QUw0TnccefdinPMREQWOQuvKxUR80k8571429H/bByBPoZxap0euh4+a/1uRx52zrlh4E2yZq375LulCJiVlTNVDXd0opnWWPJv0FrPCXNHImn2urmdi64CJpbkRx59vFNg70X/oUtnABND7muvvaEkq2TaOv/XfYROrFnWO6oYGoZPvndLa31dyjNj52hqslynnM9jrj7nBTABQAiUjSAc89BDjwr+Y8W2IZZTxAvuQE+o3jNuFonWPWL5I7QTiyeuH7gXPfzIY3L7HXdL/+sGytVX9836ctHXZk+C4VcGD5HGpiZXZCs1wJw5a7YCOGjspEumzwAi4ErjMxnfSTuHYYE9c6Tmza4TlW3tdgUw8ftk/lOtnXRjcK4pPW7EeN1/PnMu/WLd6WPSXY/vOwOYLDjUIDffbOVlSL5+gs728z/wxpuVDhKviSeeeFoef+Ip5UFConBd9UDNh4scuBzHuBYtWuxq3XtxUF4AE3GcSUsmZvL/LN6ystFejKtbrrFx0ya5fuDNapEyNr1YmFR8+4YPf13IgLR69RrB7QJrOq5HOLCT+gwfQTjVESPK5K6773XlzsJ9eChYiOs3bHQ17lICzEWLFiXcbJLXU6r/mQvW2cCBN8uTTz0rGKVmV9XIihUrZf369bJhw0ZZt369rFy1SubMnSfjxk+Ql18ZojL/ZwLNzgImOkCMO+j/UvU3+Tu9prjfffc9qJLO4JGxcNFi5XZD/9ev36A+L168RKZPr1T5V0lQk6n/3KezgMmiozorL27dX+iMem3ADTeqOlSsaVQm+GHv3btP+eYydtqBgwdl85YtKncDbofXDbBckPS1Mu15yeBjmi/Xw5wDJmDw6GNPKNY708BZCLzVu0OR6wplXB6EGw2WPB5KxosT7qxZVbJv3z6XV7AOAzyxOl7b7zqLA8rwxuVBQKwvHzvO1T1KBTC3b9+hnJ41rTOuL9v3Ep/Ad997X9asWeNaT473A9xQpvt0BjCbW1qUUzx66GxgxtiQwHhOAFjKRRMR5nbDbYs+ZrpPVwBzz969SkKCMbry6r5y6613ytvvvCcrV67qUN5TvGnmzZuvHNoz0VvPNcDMnPK85GPLOWDyxkMRz2TpQabasxgeePARIet6MW88xCTwQORg4rtivCL65a233nGlzmCRPvvs864s5qUAmNAGzs+N5MLa48EaPGRYpyqCNjQ0qoigTA9wZwCToAYAMNuzAcgxTqJiSF/YmdDBuXPnqXvlCjB5ZonMYTwAZVeL9cEl33HnPWrtp8IL/R3jYW5nz/bejzcVDuUcMElgAREzTRSDZ0FgIWxubknVz6L5DtEAFyOvgJ+wQcCXF4peJKn2LBr804hFz7aVAmBWVdcooGFtpaKH/g66ILlMmDipUz6r0DIX2YoIcYRByATCjMECSxLPPCc7d+7MNrVpf89HmV3KqaDa8CoZNrW9mN9scwwN33773UTsf1oiePBDTgET7opaLrxBMwEmvwEIZWWjPBhS6V1iRuVM5cCciYZ9+16nFP6rVq7OSoBiB0zABiNJNrCxOLeBUlk5KytNMh2QC8AEwJ06Pw3wyXsYCcIL0XF3ZcsHYHalf6nOVev0ycyqEOjFSxHjKdJdrrecAiaZeJ5//iUbMNOXngAIWNwzZ3ZtYeeaWN11fXJ/YlXPJLppGiLmZduKHTArZ85UD0kmzkO/hMeUu9PrZqKZ14B5uP6wkgaySQ28EPCk6IiuMt04ihEwGUt5+TilE83ELPBcUI4GfWmut5wCJiIE1rlMDzpvCKJ7WPwLFy7K9XiL8vpuAI4FxZsWY0C2zc31uNbtt98lO7ogBtIPr0MjyX5EJA8W2GRuzPk/YEMmfY7v6uY1YFbOnKX6nw0EcG9atnx5V7uvzi9WwMQOgKtdppej/m1BHvAjp4BJMt6bb70j42AtwLT0UNT3MduRFGhtaVEOupkMHBow3XDpxQyYy1esUKqHTC9hHiD8EXHf8mLzEjCbm5uU5wSWcSfAJ39mrt9++z0vuq+uUayASSSR5aOZ3mjM2mc9YNjK9ZZTwFy/YYMMuOEmV4B53YAbZaUL/VuuCVKo1x8ydHhGi7AGTCJesm3FDJgjy0ZlpAPAA9hAL69887wEzK1bt6o46EyAz2+AxKZNm7NNpevfixUwt2/frrxOMtFLAyZ+s7neCggwB3ZbBpJcEznT9XmoY/G4hMJhFeGEpZGYYvzaiJNFkU0kBSm+Mhk5egJghsMRJY5nogPcJY2EtV5tXgLm7NlVWcVxAP+VwUM9zbhfiIBJWCUZ73Fer/f5VCAHYdR4ehDYQaMQIS8PLXYnc+L83yMBk0EvXeaNvsarB8XL67A4du/Zo9wupk6brpyn4YKobfTkU88o537CQnUWasIkUWSTleX6G27MKL7pRYPesZQ5TOKtcVJmnKkeHL7jt7vvuU9Fkng1f14C5utvZI560w//rFneZuvqTsCkQiuRPET6EFwBDV5+ebBywCc0Eo8HKg/orPEYOPHdJoM9LmHp5lp/r2lW9BwmIoVyWs/iK8eAcYuZ78LC69VDkI/rtLS0ypatW4UaRhggAEBECzgkQrquuPIaJT7yPw2rKY2HnsaxNN6u0EgvkFR7fuecUgZMElATI52J24COQ4YM95Q78wowiWJ5/PGnMkoKzLfX4jhrPd+ASYTfokVLVODFQw8/puaNJBtwz4SBsv75rNa+I7l0qrWfar07vysZwNyxY4eKoWYROAeY/Fk/7NOmzcgHjuX8HjjuEuLJg3vjTbeohYHPna55ng38kunj5n9Nw1IGzIqKaSqzTTr69etv6S+J//Zy8wowCWYgpwDAkW5OeWHiSnTQ44i3fAGmz+9X9YYeevhR9bIHHAFF/dJPN3fp6OHme64JxhQ9h7l//3555pnn1GAyDZwBk+bpgw9HernOu+ValEWgTC/j5S3KROZikSTTk3uUOoc5avQY9fJJHrv+X62jq/uq2H0vJ98rwNy8ZWtCytB9Tt4DLs8//2KXQmpTjT0fgInIDdjDHGiQTB5fLv5n3ksCMGHLCVlyAxqw5y+8+LKg6yvWDbcoqguSfIMx52JxpLtmTwBMqjeyTjLRALrPnTvf0yXkFWCuWr1GuTtlUikwPnTbXpdYziVgEts+cuQoxUXSf9ZiujnKxfclA5itrS0yY8ZMVfUwGxERRXBy9yKqwdOnxeXF0NegpM70QGdaLNBHNx4oZwMEMj1kXJdzS53DfPOtdzLSFxpBB1Kdebl5BZg4oTNXmeaS9fPqq697Fo+t6ZArwAQs9bx0hknQa17v2697KzEJv2V7drh30YvkTBbhSlh5My0S5yJaunSZnuOi2WPcwqCTLfpETzq0ANwQW3hAaLwwru3bPvM0FsLrr79R5W7EEVufn2rPourpgAkNeHDchId2ZHF5BpjL3AEmiai98iHV48wFYNLHslGj1frN9nyzZq35GaDWul73PAOs2wRT0P96ZRknyolKAm4s5XreSwIw4RhxF4AoqR5053cQj2QdxbThR/b0088pi7dzLKk+s6i0bodUXS+88LKyIlIKlkJSUyqmyozKSqmuqlZvSzgluBJSXY0eMzbBgaa6dk8ATHKN8qClGj/fQQPo63XJXq8AE+YBIMgELvQf1RT39HLLBWCSoJiXuhvO0mIQ+ikApBQHL4X33v9QFXybOGmKYPAlc35NTa164S1evFRQYRDZRWKNTPhRUoCJxZgsypkWun4A4LLuf+AhITlBsWyEIpK8l0nT40i11xNODse58+YJxiFyOrrddO7EdPfhe+5RylbyESPLMq4jaMA6mzhpsluyujrOK8AkN2y2MD+eAdL5dTU7UfLAvAZMxSg8MyjjfOiXGGMityVGO0IdSVnoloNGl8sLhBdJqudK36NkRHImbtKkKWrA6R52TQj90HvttJu8eLz6n/R1lDnIJooDZLfedofU1NZJY2PnjFq1dXMUZ5KOhpp2pQyYcOEkSk5HA9YRv7/++pue6gC9Asxdu3cr4NAvT73unXsefBJQb9u+3atlqq7jNWBSR4dSEpm4ZeYJsESS6mzqNZ4x0tv1KMAkphwdXCbi6kUDYR597ElV58bTFZODi61du14GXH9TRpGEwk6MvatO+QYwRaprahVYZlpHPKAk5g2GQp7NuFeAGQqFldsNfdTrPXmvM3fNmeNtXLSXgInUiPdLNqmRcVIy2+frfFHDHgmYiJ5PPfNcVgKzePRbaUrFNM8WfK4uNHlKhXqDZuJ4eAEQCibS2qVuGMAUVZwsm0gLmJLIZdUqbzIVMWleASZAQyhsNqDh9zfefLtL6yX5ZC8BUydwzgz8A5S+dlkXw517JGAyedOmz1ALJRO46LctIgtWZzK7FPKGg3qmxc/Dy3gXLuy6m4sBTJGDB4mUeSBjpAxriDnBoODV5hVg0p8xY8ZmfQ5Y/3fedY/s3bvXqyF4GhpJUoxbbr09oyEGMCVGnMxYXdl6LGASFuamqJEGTfIFkigW5/dC3FBGo1vJBJhaH7XRgzRdBjBFmhqbZMjQYRlpzvoBcEjgsGvXLk+WjpeAuXjJUtW/TIwDv6EXHzd+oif95yJecphYrwcOvCWjKgrJCkt4Vx3weyxgMmmkmycKRoNipj2LhtDCYcNe7ZA12bMVluVCqBmefmZQRmU0gMmDu2XrtixXy/6zAUyLRkgqcC+ZAId1xYvszbfelpbWrqlCuKuXgImFmGxKmcRZDfq3kvF+R+cLnzlXlZeAuWjxYlfuUW+88VaXjW89GjAtLvPurItFA6kGzddee8Pz2FrnYurMZwWYT2fOUYlIfv3Am2TN2nWduUW7c7oFMO+4q0uVChmA1yUqqD99+x13ZRQHWT9aHVI3Z247OnbmHy8Bk/sTGQMzoNd5uj1SFi41HXE/Szc+TwFzkTvAHPT8S67KPqfrM9/3aMCEADNmVGbV4TgXkAbNF198WfkuZiKuF79FozFZs2Zt1trPiBqWAj+zfxgZityUjcjW9/wD5nUq9+S2bV1zb/EaMPHfe/NNQiTT012vH7g4dG1dLY7lNWCyvvplCZFkDKx93KTef//DLou2XgKmEslvzCySoxYhOOPAwYPZlnbG33s8YMbjDQpo3Lxh9cJXC+eqa5XCv7a2zpM3bvIsMTEYZ555dpASN4iuybZlSwZL/9HlPDfo+S732S1guilo76ZEheKOb7hRVq9Zm40MGX/3GjC5GXNzY5YHVq8d6E/SYfwGO7t5DZhNzc3y8stDXHGZzAPA/84773fJgOIlYJLIGaMsoKjpnLyn36ikiN7pyuYWMPv1G+B5SGyqfue0REWqG/IdXAsVCVnMyYTO9D8Lh0lATCEsKxyJpLuFq+/hVnbt3iPU/aYEhMXNXqvCHMvKRme9RsXUaXLNNf0z6tO0aDhzZtcWjttIn+rqmqz9dgOY1kuqr/JuyHrBDAfkAjCZN+0HSD8zrRl+IxILX1jCSzsTQdPQ0KjCXzOtV9Ylcc8bN27KQI22n4h4IUyS87L1nzUER/3MM4NUpEzbVdx/okCYXovp7nfl1dfKhy5SLAYCAVUhgJSM6a7F9zyvZFLft3+/+44mHUnS5WyRPoyL++Wj6my3ACY0IQUXg8z0lko1GXrxEGWAFX3ChElKR4jLCcTNtKELIrYdDgXjAVY8y3JvJcLQhGei73/g4awOt+vWrVcP4rVZMsozxltuub3TE9rQ0KBizTMteICD+8BJZNsswHw66wuL60GHDRs3JkRCVBH44REa52bLBWByX0JL77nn/qxj0GsIYALwyP5NiRDqxrgNz4vGYvLEk5np1VHAZAwjRpQpLtMN6FsvsGtVyRJeFmvWrJFI2N0ccC9q49DHTPdyC5j4k777XuZUe5ruV19jxcYf7KRovnPXbgXOPJP6msl7novrr79JMVFu1mRXjuk2wKTTk6dMVZOIni+ZCNn+h0g8ABAS7uGBBx4W4rTfeec9GT2mXIVjTp5cIWTfJlcfei/eVPff/5DiBAADLKnskxcR12ZxZYu2QFygHk8mzkOP4+pr+6vFPnbsBNm7d19G6yHXpRAaZWInTpys3Jeo7ZPcT31t9vxGn93kglTFxFz2G/og0iJCYnwbPHiYPPzwYyrqxs3CyxVgcm+rZvUNatxOWqT7DI1YLzRelEOHDldx50uWLFU1ZwhdxPcRMKW0CDWmkCKGDX9NlVphXaS7dmcAE06N9dMR9RT3Yd1iTHzq6efkwxEjpaamTnGe1JDnRcIYdu7cKWvXrVPJLADmBx96NOP6YVxuARPaL15iJd7IRBOuqWlO9B4qr0x+mU3NTQJNEPnnzpuvQJkXdjqa6+/pAwENy5blvkx3twImb3gAjUXAg6kJ0JE9EwLBdL2QK+1sz/zPNRHHWGA6CzTfcT/Oy3QfzqFyXzYuBFGbe2RbONyLFwP9gjNC/4mfHWUXJk2eIuPGTVCpsrCgDhr0ojoGEY+HG3Es2/UZD+NyE4JJkuYXXxqs6JKJBvo3/ZBCE9rfL79SLea/LBjBAAAH3UlEQVTuBkzuDw2hEX3U/c22h1asA8bCuYjG1J667fY75Y4771auYDfdcpt6Eevjsq0X7t8RkVzTbuvWbco4Ql+y9dv5OyG3vKg5j77xQkWvSPE8ZwE9fuMYxuE8P9XnjgAmL3X0/a763f8GVVGBzEZIhQA4a56X0YSJk5S7IS/WocNflccee1Juu+1O1Veuzfy4oT1j76pxT89Jpn23AiYdA5AIMYSYbji1VBOd/B0Edrbk3938zwIjCcLWbZl9KHXpV7dcAv2yHkJd+Ky/DYoWGGEVBYA5hocw22LRYwFQaW6T57773vvyjyvd+cTqe+g9C/nFF1+R5uaWTGtL/ZZLDpMbYEBBgmDtdAQ09VigL3TjXGiuG//zvVv6c3xnAJMxIEkAcq7A57objgA+3X/nGPisx6DHmm3fEcCk3zjhoxpzKyHST4sBsIAQWmvQZ+x85ne+59hs/dW/M84777pX1qztmoFSLdgsf7odMHX/UErfeefdihPsCLE00bze86D8/fKrlKFA9zHdHh0fHEpHAZ97OFtXxqBptsRlAma8DViYbgHB2TcWNSnI/C6SKuQaMJkT9Kpw6NCgo3PgHFdXPvPQdhYwGcPadetV0hBevHouu9KfzpzbUcBsbWmVESNH2c9sZoktuT/OdU/xuuTfO/I/tKc8Nc9hrreCAUwGiniiLWJY4DrzMHeE0OmOBRBYuHffc7+4dXxesGChDBhwY6e5hHR9cfs94MfCcVvb/dDhw3L3PQ90CmC4D3rNLVu2ZF2f+QBM3QkMXnhfEE2Wb9CB/v2vG6gMiro/Hd3v2bNHXnllqJIwWINu596r46Dbu+++36FuR6JRGf7q68qzJN80Z9xgBNwp5W2ofJDrraAAk8FSIwTndsAKQrBw8gGc3AOQBiiJJBk1urzD9YXw9UO0yvUDS19ZnDyk0IjG/7hwZFMhOBcU1mJ9bkceOu5FGjJC5LJt+QRM+oLBAO8H/PLywa3pdXP5FVcpXeTOLsav48kxffoMJWLS/1yvf/rPC1DfC0NpRzf0mah4WI+55vD12tdMDXt00ORAzUc9sIIDTD1ZDB6DkAZOjDZMLATryMOd6ViuxSSzWPCnxCLHPfft26e70eE9sb9DhgwTlPLUZO5qn+kjjeuwGOkre66PgeL5F15WaoPly5fL4cOHM1rfkwfDw0nGJQCe62eilfM3jsXwQ2mBbBtZgwATPY7kPeCL3pZcl15t6MWXLVsmL770stKNa5pxL+7vHEtHP3M+19FzwfqhSsCY8nFC2CYuN15s+/btV94e6OZ4qTGGrq4lxqrpr9c918bCzJrFt7mzYZjQHDXPvfc+oNY99OkKrXU/oTWgSD9pGiBxD3vt9TelsnKWymoGaOdjK1jA1IM/dOiwqjyJRQ4dkSYchhG9gLJNjJP4LBQeUAAY4sNNDh48VObMmSt+f+cTner+skentmjxElWaI1Wf9YPr7BffMR6s6Cy2tnFaRaJuvvV2eeTRJ2TwkGHKwrhi5UrlnsS9urLh/I+zMkY3C1gsuuo+9m3HzVqATb+ffXaQqxh5IlT++rfLFa2hd3JjrH+/4irP6/BAE9xUCEMEtAE1uGInXaE1NGesifH2swBRzwfrJXk+oBVJislZuXDBIs/WTap5xL1s+vRKefa555UlnLWrxwAt6Z+z/8yN/p+x8TvHcQ5rnvP5DkmI+ucVFVNl+/btngE9L+3xEybK/Q8+rO6T6Kuj0Jle97qvR/QTcLQNQIA8XiUEluAyyIsV0Rv3o+7YCh4wNVGampqVzgxxnbrN+JVhaGHycbuxmm1ptt9GerLYqzfedderhYITMroaqszhE5mrjTLDW7ZsVe4Tr7wyRGXbxv2BRaAXiV7wiBXEPd9z7/2q6BN1kD74YISKQlq+fIXgY4e+KFfbunXrVPTMgw89IjfdfKviDugjnCx9w28Rn8GRZaMUCGULEtD93LZtm/KpIwojXcOfcv/+A/qUnOx5wIiBplwKLx0KayG9MNbr7LhuNd5+lhTDmPGSIB6aol3kDQB4q6prlJ4y32kHm1uaBVpS5gTggIHA95hsWFQWBXys+WoryUz4KMCI5ITP5vDhr6uX7dKly7P6And1EoLBoODfyssYsIPWuD4B5qx5Gv2l3zAV9JPxUFAQv9exY8crJmb9uvWC03tXGYOujkefXzSAqTus9yxYjA640VTOnCXlY8fLe+9/oPwbUULDrhMRQRVK3JZw7l2xYqVySiZypju2YDCkwkJXrVqtHH9x5KWeOYYaOCGMXiwO9LjdtVHaYfPmLQJII6JR9phsSzhEuwXJ7up7R+5LfDiJIRgr80F1TtxkGO/y5SuViwrzgWgcieTuRdWRPjuPBUCQiHiREma5fMVKwUMCkGIsjGnjpk0qJyglIroTcPD7JRKPsFH6hq6ftU9feSYpYYM6g/F0Zz+d9E33uWgBM92ArO+90SNlvof51VDAUKCnUaBEAbOnTaMZr6GAoUA+KGAAMx9UNvcwFDAUKAkKGMAsiWk0gzAUMBTIBwUMYOaDyuYehgKGAiVBAQOYJTGNZhCGAoYC+aCAAcx8UNncw1DAUKAkKGAAsySm0QzCUMBQIB8UMICZDyqbexgKGAqUBAUMYJbENJpBGAoYCuSDAgYw80Flcw9DAUOBkqCAAcySmEYzCEMBQ4F8UOD/A0m8oXKVViY0AAAAAElFTkSuQmCC'/>
                    </div>
                    <div style='margin-left: 40px;max-width: 800px;margin-top: 60px;'>
                        <p style='color: #868685;font-weight: 300; line-height: 16.8px;'>Estimado Usuario,<br/><br/>
                        Se le informa que se ha realizado una modificación en la sección "+modulo+@", a continuación, se presenta el detalle:</p>
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
                        <img style='zoom: 1.5;' width='89' height='75' src='data:image/png;base64,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'/>
                    </div>
                    <div style='margin-left: 40px;max-width: 800px;margin-top: 60px;'>
                        <p style='color: #868685;font-weight: 300; line-height: 16.8px;'>Estimado Usuario,<br/><br/>
                            Se le informa que se ha realizado una creación en la sección "+modulo+@" con el codigo "+codigoCreacion+@".</p>
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
