using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ConfRolesPerm
    {
        public int cod_perm_campo { get; set; }
        public int codSec_permViz { get; set; }
        public int cod_seccion { get; set; }
        public string nom_seccion { get; set; }
        public string modulo { get; set; }
        public string nom_campo { get; set; }
        public bool visib_campo { get; set; }
        public bool edit_campo { get; set; }
    }
}
