using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ComentarioPY
    {
        public int Id { get; set; }
        public int TipoSeguimientoId { get; set; }
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public string? Descripcion { get; set; }
        public int TipoComentario { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
