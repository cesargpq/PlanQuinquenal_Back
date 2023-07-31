using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class TrazabilidadDetalle
    {
        public string Id { get; set; }
        public int? TOTAL { get; set; }
        public string? Tabla { get; set; }
        public string? Evento { get; set; }
        public string? DescripcionEvento { get; set; }
        public string? DireccionIp { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string?  Usuario { get; set; }
    }
}
