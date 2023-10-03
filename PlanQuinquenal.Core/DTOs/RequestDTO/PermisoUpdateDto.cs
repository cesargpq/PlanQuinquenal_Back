using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PermisoUpdateDto
    {
        public Decimal? Longitud { get; set; }
        public int? EstadoPermisosId { get; set; }
        public string? Expediente { get; set; }
    }
}
