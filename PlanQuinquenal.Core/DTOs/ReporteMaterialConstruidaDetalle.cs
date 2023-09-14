using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class ReporteMaterialConstruidaDetalle
    {
        public string Distrito { get; set; }
        public Decimal? LongitudAprobada { get; set; }
        public Decimal? LongitudConstruida { get; set; }
        public Decimal? longitudPendiente { get; set; }

        public Decimal? Planificado { get; set; }
        [NotMapped]
        public List<string> categorias { get; set; }
        [NotMapped]
        public List<Decimal?> pendiente { get; set; }
        [NotMapped]
        public List<Decimal?> construido { get; set; }
    }
}
