using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PQuinquenalReqDTO : PlanQuinquenal.Core.Entities.PlanQuinquenal
    {
        public List<int> IdUsuario { get; set; }
        public List<PQUsuariosInteresadosDTO> lstUsuaInter { get; set; }
    }
}
