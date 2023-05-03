using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    [Table("perfil")]
    public class Perfil
    {
        [Key]
        public int cod_perfil { get; set; }
        public int cod_rol { get; set; }
        public int Perm_viz_modulocodMod_permiso { get; set; }
        public int Permisos_viz_seccioncodSec_permViz { get; set; }
        public string nombre_perfil { get; set; }
        public string estado_perfil { get; set; }
        public TablaPerm_viz_modulo Perm_viz_modulo { get; set;}
        public TablaPermisos_viz_seccion Permisos_viz_seccion { get; set; }
    }
}
