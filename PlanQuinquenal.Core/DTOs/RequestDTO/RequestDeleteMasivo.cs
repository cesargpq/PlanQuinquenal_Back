using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class RequestDeleteMasivo
    {
        public string base64 { get; set; }
        public int CodigoPlan { get; set; }
        public int TipoProy { get; set; }
    }
}
