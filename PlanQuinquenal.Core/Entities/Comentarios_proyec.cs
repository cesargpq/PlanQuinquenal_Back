using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Comentarios_proyec
    {
        [Key]
        public int id { get; set; }
        public int id_pry { get; set; }
        public string comentario { get; set; }
        public string tipo_coment { get; set; }
        public string usuario { get; set; }
        public string area { get; set; }
        public DateTime fecha_coment { get; set; }
        public int cod_usu { get; set; }

        [ForeignKey("cod_usu")]
        public Usuario regusuario { get; set; }
    }
}
