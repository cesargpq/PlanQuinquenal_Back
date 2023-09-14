using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class Serie
    {
        public string type { get; set; }
        public string name { get; set; }
        public List<double> data { get; set; }
    }
}
