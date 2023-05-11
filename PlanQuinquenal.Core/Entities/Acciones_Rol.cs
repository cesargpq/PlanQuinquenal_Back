using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Acciones_Rol
    {
        [Key]
        public int id { get; set; }
        public string modulo { get; set; }
        public string unid_neg { get; set; }
        public bool acc_ver { get; set; }
        public bool acc_crear { get; set; }
        public bool acc_eliminar { get; set; }
    }
}
