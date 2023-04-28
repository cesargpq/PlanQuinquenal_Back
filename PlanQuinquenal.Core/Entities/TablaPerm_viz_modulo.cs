using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class TablaPerm_viz_modulo
    {
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
