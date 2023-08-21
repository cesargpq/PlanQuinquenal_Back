using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    [Table("Constantes")]
    public class TablaConstantes
    {
        [Key]
        public int id { get; set; }
        public string codigo { get; set; }
        public string? valor1 { get; set; }
        public string? valor2 { get; set; }
    }
}
