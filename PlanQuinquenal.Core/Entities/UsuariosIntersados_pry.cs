using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class UsuariosIntersados_pry
    {
        [Key]
        public int id { get; set; }
        public int id_pry { get; set; }
        public int cod_usu { get; set; }
    }
}
