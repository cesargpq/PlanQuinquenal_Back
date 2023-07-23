using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class CamposModulo_Permisos
    {
        [Key]
        public int id { get; set; }
        public string descripcion { get; set; }
        public string nombre_campo { get; set; }
        public string modulo { get; set; }
        public string pagina { get; set; }
    }
}
