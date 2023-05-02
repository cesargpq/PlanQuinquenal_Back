using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    [Table("Perm_viz_modulo")]
    public class TablaPerm_viz_modulo
    {
        [Key]
        public int codMod_permiso { get; set; }
        public bool perm_dashb { get; set; }
        public bool perm_planQui { get; set; }
        public bool perm_proyectos { get; set; }
        public bool perm_gestRem { get; set; }
        public bool perm_infActas { get; set; }
        public bool perm_reportes { get; set; }
        public bool perm_admin { get; set; }
    }
}
