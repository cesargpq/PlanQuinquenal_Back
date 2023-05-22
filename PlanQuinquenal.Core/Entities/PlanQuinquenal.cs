using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PlanQuinquenal
    {
        public int Id { get; set; }
        public string Pq { get; set; }
        public int EstadoId { get; set; }
        public DateTime Aprobaciones { get; set; }
        public string Descripcion { get; set; }

    }
}
