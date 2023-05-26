using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
                    flag_visto = notificacion.flag_visto
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

        public Task<object> EnvioCorreoNotif(CorreoResponse correo)
        {
            throw new NotImplementedException();
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
            var queryable = await _context.Notificaciones.ToListAsync();

            return queryable;
        }

    }
}
