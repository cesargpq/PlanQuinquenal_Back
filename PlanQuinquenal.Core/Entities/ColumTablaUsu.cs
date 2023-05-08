using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ColumTablaUsu
    {
        [Key]
        public int id { get; set; }
        public bool seleccion { get; set; }
        public string title { get; set; }
        public string campo { get; set; }
        public string tipo { get; set; }
        public string correo { get; set; }
        public string tabla { get; set; }
    }
}
