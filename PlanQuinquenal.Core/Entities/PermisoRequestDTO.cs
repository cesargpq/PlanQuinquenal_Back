using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PermisoRequestDTO
    {

        public string CodigoProyecto { get; set; }
        public int Etapa { get; set; }
        public string TipoPermisosProyecto { get; set; }
        public Decimal Longitud { get; set; }
        public int EstadoPermisosId { get; set; }
    }
}
