using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Imped_gest_inast
    {
        [Key]
        public int id { get; set; }
        public int id_imp { get; set; }
        public string nom_doc { get; set; }
        public DateTime feha_reg { get; set; }
        public string mime_type { get; set; }
    }
}
