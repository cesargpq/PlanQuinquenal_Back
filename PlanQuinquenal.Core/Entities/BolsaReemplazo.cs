using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class BolsaReemplazo
    {
        public int? Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public int? DistritoId { get; set; }
        public int? ConstructorId { get; set; }
        public string? CodigoMalla { get; set; }
        public int? PermisoId { get; set; }
        public int? Estrato1 { get; set; }
        public int? Estrato2 { get; set; }
        public int? Estrato3 { get; set; }
        public int? Estrato4 { get; set; }
        public int? Estrato5 { get; set; }
        public Decimal? CostoInversion { get; set; }
        public Decimal? LongitudReemplazo { get; set; }
        public string? RiesgoSocial { get; set; }
        public int? ReemplazoId { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModifica { get; set; }
        public int? UsuarioRegistra { get; set; }
        public int? UsuarioModifica { get; set; }
        public bool? Estado { get; set; }
        public bool? Reemplazado { get; set; }
        public int? NumeroReemplazo { get; set; }
        public DateTime? FechaPresenacionReemplazo { get; set; }
    }
}
