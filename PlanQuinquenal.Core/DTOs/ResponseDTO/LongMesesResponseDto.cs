using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class LongMesesResponseDto
    {
        public List<string> categorias  { get; set; }
        public List<Decimal> enero { get; set; }
        public List<Decimal> febrero { get; set; }
        public List<Decimal> marzo { get; set; }
        public List<Decimal> abril { get; set; }
        public List<Decimal> mayo { get; set; }
        public List<Decimal> junio { get; set; }
        public List<Decimal> julio { get; set; }
        public List<Decimal> agosto { get; set; }
        public List<Decimal> septiembre { get; set; }
        public List<Decimal> octubre { get; set; }
        public List<Decimal> noviembre { get; set; }
        public List<Decimal> diciembre { get; set; }
    }
}
