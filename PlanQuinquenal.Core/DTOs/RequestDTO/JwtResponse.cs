using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class JwtResponse
    {
        public string AuthToken { get; set; }
        public DateTime ExpireIn { get; set; }
        public string Message { get; set; }
        public bool state { get; set; }
    }
}
