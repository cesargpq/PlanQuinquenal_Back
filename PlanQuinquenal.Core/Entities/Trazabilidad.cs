using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Trazabilidad
    {
        public int Id { get; set; }
        public string? Tabla { get; set; }
        public string? Evento { get; set; }
        public string? DescripcionEvento { get; set; }
        public int? UsuarioId { get; set; }
        public string? DireccionIp { get; set; }
        public string? CampoAnterior { get; set; }
        public string? CampoActual { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
