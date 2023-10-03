using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class RequestPermisoGetDto
    {
        public int? Total { get; set; }
        public int? PermisoId { get; set; }
        public string? Distrito { get; set; }
        public int? ProyectoId { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? Expediente { get; set; }
        public int? TipoPermisosProyectoId { get; set; }
        public decimal? Longitud { get; set; }

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? EstadoPermiso { get; set; }
    }
}
