using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
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
        Task<Object> CrearNotificacionList(List<Notificaciones> notificacion);
        Task<Object> EnvioCorreoNotif(List<CorreoTabla> lstModif, string correoUsu, string tipoOperacion, string modulo);
        Task<Object> EnvioCorreoNotifList(List<CorreoTabla> lstModif, List<string> correoUsu, string tipoOperacion, string modulo);
        Task<Object> ModificarConfigNotif(Config_notificacionesRequestDTO config, int codUsu);
        Task<Config_notificaciones> ObtenerConfigNotif(int cod_usu);
        Task<PaginacionResponseDto<Notificaciones>> ObtenerListaNotif(RequestNotificacionDTO r,int cod_usu);
        Task<List<CorreoTabla>> ObtenerListaModif(int codNot);
    }
}
