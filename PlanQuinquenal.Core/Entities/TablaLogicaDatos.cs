using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class TablaLogicaDatos
    {
        [Key]
        public int IdTablaLogicaDatos { get; set; }
        public int TablaLogicaId { get; set; }
        public string? Descripcion { get; set; }
        public string? Codigo { get; set; }

        public string? Valor { get; set; }
        public bool Estado { get; set; }
    }
}
