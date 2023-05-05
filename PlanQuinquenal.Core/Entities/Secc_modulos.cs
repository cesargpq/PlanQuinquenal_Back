using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Secc_modulos
    {
        [Key]
        public int id { get; set; }
        public string modulo { get; set; }
        public string seccion { get; set; }
        public bool vis_seccion { get; set; }
    }
}
