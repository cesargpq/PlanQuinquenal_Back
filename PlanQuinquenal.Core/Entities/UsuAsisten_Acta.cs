using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class UsuAsisten_Acta
    {
        [Key]
        public int id { get; set; }
        public int id_acta { get; set; }
        public int cod_usu { get; set; }
    }
}
