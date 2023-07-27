using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ListaPqMensual
    {
        public string type { get; set; } = "column";
        public string name { get; set; }
        public List<decimal> data { get; set; } = new List<decimal>();
    }
}
