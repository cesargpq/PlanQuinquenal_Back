using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Secciones
    {
        [Key]
        public int cod_seccion { get; set; }
        public int nombre_seccion { get; set; }
    }
}
