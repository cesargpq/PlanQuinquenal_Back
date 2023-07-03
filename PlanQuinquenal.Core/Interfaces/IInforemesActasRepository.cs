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
    public interface IInforemesActasRepository
    {
        Task<ResponseDTO> Crear(InformeReqDTO informeReqDTO,int id);
        Task<ResponseEntidadDto<Informe>> GetById(int id);
    }
}
