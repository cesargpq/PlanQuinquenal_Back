using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class GestionReemplazoDto
    {
        public List<int> Impedimentos { get; set; }
        public List<int> BolsaReemplazo { get; set; }
        public DateTime FechaPresenacionReemplazo { get; set; }
        public int NumeroReemplazo { get; set; }
    }
}
