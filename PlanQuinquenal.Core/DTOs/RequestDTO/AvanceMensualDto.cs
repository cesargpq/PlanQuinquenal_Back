using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class AvanceMensualDto
    {
        public int TipoProy { get; set; }
        public int CodigoPlan { get; set; }
        public string? AnioPQ { get; set; }
        public int MaterialId { get; set; }
    }
}
