using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class CorreoResponse
    {
        public string asunto { get; set; }
        public string mensaje { get; set; }
        public string correos { get; set; }
    }
}
