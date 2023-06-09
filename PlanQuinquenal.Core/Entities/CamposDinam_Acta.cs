using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class CamposDinam_Acta
    {
        [Key]
        public int id { get; set; }
        public int id_acta { get; set; }
        public string nom_campo { get; set; }
        public string valor_campo { get; set; }
    }
}
