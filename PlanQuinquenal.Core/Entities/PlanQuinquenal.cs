using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PlanQuinquenal
    {
        public int Id { get; set; }

        [Description("Código PQ")]
        public string Pq { get; set; }

        [Description("Estado ID")]
        public int EstadoId { get; set; }

        [Description("Aprobaciones")]
        public DateTime Aprobaciones { get; set; }

        [Description("Descripción")]
        public string Descripcion { get; set; }

    }
}
