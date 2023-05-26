using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryNotificaciones
    {
        Task<Object> CambiarEstadoNotif(int cod_notif, bool flagVisto);
        Task<Object> ContadorNotifNuevas(int cod_usu);
        Task<Object> CrearConfigNotif(Config_notificaciones config);
        Task<Object> CrearNotificacion(Notificaciones notificacion);
        Task<Object> EnvioCorreoNotif(CorreoResponse correo);
        Task<Object> ModificarConfigNotif(Config_notificaciones config);
        Task<Config_notificaciones> ObtenerConfigNotif(int cod_usu);
        Task<List<Notificaciones>> ObtenerListaNotif(int cod_usu);
    }
}
