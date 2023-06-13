using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class UsuariosIntersados_Inf
    {
        [Key]
        public int id { get; set; }
        public int id_inf { get; set; }
        public int cod_usu { get; set; }
    }
}
