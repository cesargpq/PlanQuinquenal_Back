using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int? iduser { get; set; }
        public int? idColum { get; set; }

        [ForeignKey("idColum")]
        public ColumnasTablas? columTabla { get; set; }
    }
}
