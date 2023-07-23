using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ImpedimentoRequestDTO
    {
        public string codProyecto { get; set; }
        
        public int ProblematicaRealId { get; set; }
        public Decimal LongImpedimento { get; set; }
    }
}
