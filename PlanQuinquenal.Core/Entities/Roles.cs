using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Roles
    {
        [Key]
        public int cod_rol { get; set; }
        public string nom_rol { get; set; }
        public string estado_rol { get; set; }
    }
}
