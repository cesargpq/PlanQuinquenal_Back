using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PlanAnual
    {
        public int Id { get; set; }
        public string AnioPlan { get; set; }
        public int EstadoAprobacionId { get; set; }
        public DateTime FechaAprobacion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModifica { get; set; }
        public int UsuarioRegisterId { get; set; }
        public bool Estado { get; set; }
        public int UsuarioModifica { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public List<Proyecto> Proyecto { get; set; }
    }
}
