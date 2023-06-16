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
    public interface IRepositoryUnidadNeg
    {
        Task<Object> NuevoUnidadNeg(Unidad_negocio nvoUnidNeeg);
        Task<PaginacionResponseDto<Unidad_negocio>> ObtenerUnidadNeg(UnidadNegocioDto unidad);
        Task<Object> EliminarUnidadNeg(int cod_uniNeg);
        Task<Object> ActualizarUnidadNeg(Unidad_negocio uniNeg);
    }
}
