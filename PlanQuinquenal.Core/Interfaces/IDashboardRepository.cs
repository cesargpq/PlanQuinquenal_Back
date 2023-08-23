using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ReporteMaterialDetalle> ListarMaterial(RequestDashboradDTO o);

        Task<ReporteMaterialConstruidaDetalle> ListarMaterialConstruida(RequestDashboradDTO o);

        Task<ResposeDistritosDetalleDTO> ListarPermisos(RequestDashboradDTO o);
        Task<MensualidadDTO> ListarAvanceMensual(AvanceMensualDto o);
        Task<MensualidadDTO> ListarAvanceMensualConstruida(AvanceMensualDto o);
    }
}
