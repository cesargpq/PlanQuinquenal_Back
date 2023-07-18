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
    public interface IBolsaReemplazoRepository
    {
        Task<ResponseDTO> Add(RequestBolsaDto p,int idUser);
        Task<PaginacionResponseDtoException<BolsaDetalle>> Listar(BolsaRequestList p);
        Task<ImportResponseDto<BolsaReemplazo>> ImportarMasivo(RequestMasivo data, int idUser);
    }
}
