using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class COMENTARIOPA
    {
        public int Id { get; set; }
        public int TipoSeguimientoId { get; set; }
        public int PlanAnualId { get; set; }
        public PlanAnual PlanAnual { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public string? Descripcion { get; set; }
        public int TipoComentario { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
