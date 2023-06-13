using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ReemplazoRepository : IReemplazoRepository
    {
        private readonly PlanQuinquenalContext _context;

        public ReemplazoRepository(PlanQuinquenalContext context)
        {
            this._context = context;
        }

        public async Task<ImportResponseDto> ReemplazoImport(RequestMasivo requestMasivo)
        {
            throw new NotImplementedException();
        }
    }
}
