using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class TablaLogica
    {
        [Key]
        public int IdTablaLogica { get; set; }
        public string Descripcion { get; set; }
    }
}
