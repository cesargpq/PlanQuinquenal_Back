using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class DashboardRequestDto
    {
        public int PlanQuinquenalId { get; set; }
        public int PlanAnualId { get; set; }
        public int MaterialId { get; set; }
    }
}
