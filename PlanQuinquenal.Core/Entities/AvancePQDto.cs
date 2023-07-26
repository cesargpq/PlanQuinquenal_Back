using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class AvancePQDto
    {
        public string? AnioPq { get; set; }
        public Decimal? LongAprobPa { get; set; }
        public Decimal? LongRealHab { get; set; }
        public Decimal? Avance { get; set; }
    }
}
