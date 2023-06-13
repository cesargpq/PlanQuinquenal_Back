using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IReemplazoRepository
    {
        Task<ImportResponseDto> ReemplazoImport(RequestMasivo requestMasivo);
    }
}
