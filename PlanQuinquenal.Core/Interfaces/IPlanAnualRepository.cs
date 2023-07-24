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
    public interface IPlanAnualRepository
    {
        Task<PaginacionResponseDtoException<PQuinquenalResponseDto>> GetAll(PQuinquenalRequestDTO p);
        Task<ResponseDTO> Add(PQuinquenalReqDTO p, DatosUsuario usuario);
        Task<ResponseEntidadDto<PQuinquenalResponseDto>> GetById(int id);
        Task<ResponseDTO> Update(UpdatePlanQuinquenalDto dto, int id, DatosUsuario usuario);
    }
}
