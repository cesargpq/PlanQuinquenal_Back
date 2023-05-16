using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Unidad_negocio
    {
        [Key]
        public int cod_und { get; set; }
        public string nom_und { get; set; }
        public string estado_und { get; set; }
    }
}
