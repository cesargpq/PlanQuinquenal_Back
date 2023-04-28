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
        [Column(Order = 0)]
        public int codSec_permViz { get; set; }
        [Key]
        [Column(Order = 1)]
        public int cod_seccion { get; set; }
        [Key]
        [Column(Order = 2)]
        public string nom_campo { get; set; }
        public bool visib_campo { get; set; }
        public bool edit_campo { get; set; }

    }
}
