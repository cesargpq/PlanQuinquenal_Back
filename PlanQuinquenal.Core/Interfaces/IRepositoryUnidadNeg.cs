using PlanQuinquenal.Core.DTOs.RequestDTO;
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
        Task<List<Unidad_negocio>> ObtenerUnidadNeg();
        Task<Object> EliminarUnidadNeg(int cod_uniNeg);
        Task<Object> ActualizarUnidadNeg(Unidad_negocio uniNeg);
    }
}
