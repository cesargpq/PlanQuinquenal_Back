using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PQComentarios
    {
        [Key]
        [Description("ID")]
        public int Id { get; set; }

        [Description("Plan Quinquenal ID")]
        public int PlanQuinquenalId { get; set; }

        [Description("Usuario ID")]
        public int UsuarioId { get; set; }

        [Description("Comentario")]
        public string Comentario { get; set; }

        [Description("Fecha")]
        public DateTime Fecha { get; set; }

        [Description("Estado")]
        public bool estado { get; set; }

        [Description("Tipo de Comentario")]
        public string TipoComentario { get; set; }

        [Description("Área")]
        public string Area { get; set; }
    }
}
