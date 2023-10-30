using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class BolsaRequestList : PaginacionDTO
    {
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
        public string? FechaRegistro { get; set; }

        public string? FechaModifica { get; set; }
        public int? UsuarioRegistro { get; set; }
        public int? UsuarioModifica { get; set; }
        public string? Importancia { get; set; }

    }
}
