using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Docum_proyecto
    {
        [Key]
        public int id { get; set; }
        public int id_pry { get; set; }
        public string nom_doc { get; set; }
        public DateTime fecha_reg { get; set; }
        public string mime_type { get; set; }
    }
}
