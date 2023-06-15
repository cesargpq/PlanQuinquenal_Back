using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("ID")]
        public int id { get; set; }

        [Description("ID del proyecto")]
        public int id_pry { get; set; }

        [Description("Comentario")]
        public string comentario { get; set; }

        [Description("Tipo de comentario")]
        public string tipo_coment { get; set; }

        [Description("Usuario")]
        public string usuario { get; set; }

        [Description("Área")]
        public string area { get; set; }

        [Description("Fecha de comentario")]
        public DateTime fecha_coment { get; set; }

        [Description("Código de usuario")]
        public int cod_usu { get; set; }

        [ForeignKey("cod_usu")]
        public Usuario regusuario { get; set; }
    }
}
