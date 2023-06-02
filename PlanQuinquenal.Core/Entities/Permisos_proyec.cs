using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Permisos_proyec
    {
        [Key]
        public int id { get; set; }
        public int id_pry { get; set; }
        public int cod_tipoPerm { get; set; }
        public string nom_doc { get; set; }
        public string num_exp { get; set; }
        public DateTime fecha_reg { get; set; }
        public string mime_type { get; set; }
    }
}
