using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    [Table("permisos_viz_seccion")]
    public class TablaPermisos_viz_seccion
    {
        [Key]
        public int cod_perm_campo { get; set; }
        public int codSec_permViz { get; set; }
        public bool visib_campo { get; set; }
        public bool edit_campo { get; set; }
        public int cod_campo { get; set; }

        [ForeignKey("cod_campo")]
        public CamposModulo_Permisos campo { get; set; }

    }
}
