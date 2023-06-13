using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Email_modificacion
    {
        [Key]
        public int id { get; set; }
        public string cod_correo { get; set; }
        public string cod_proyecto { get; set; }
        public string campo_modif { get; set; }
        public string valor_modif { get; set; }
        public string usua_modif { get; set; }
        public DateTime fecha_modif { get; set; }
    }
}
