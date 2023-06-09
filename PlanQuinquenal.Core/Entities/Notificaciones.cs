using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Notificaciones
    {
        [Key]
        public int id { get; set; }
        public int cod_usu { get; set; }
        public string seccion { get; set; }
        public string nombreComp_usu { get; set; }
        public string cod_reg { get; set; }
        public string area { get; set; }
        public DateTime fechora_not { get; set; }
        public bool flag_visto { get; set; }
        public string tipo_accion { get; set; }
        public string mensaje { get; set; }
    }
}
