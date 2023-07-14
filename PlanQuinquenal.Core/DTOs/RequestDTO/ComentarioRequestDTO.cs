using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ComentarioRequestDTO
    {
        public int TipoSeguimientoId { get; set; }
        public int ProyectoId { get; set; }
        public string Descripcion { get; set; }
        public int TipoComentario { get; set; }
    }
}
