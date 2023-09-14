using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ImpedimentoListDto:PaginacionDTO
    {
        public string? CodigoProyecto { get; set; }
        public int? PAnualId { get; set; }
        public string? FechaReemplazo { get; set; }
        public string? CodigoMalla { get; set; }
        public int? DistritoId { get; set; }
        public int? CausalReemplazoId { get; set; }
        public int? ConstructorId { get; set; }
        public int? IngenieroResponsableId { get; set; }
        public int? ProblematicaRealId { get; set; }
        public Decimal? LongitudReemplazo { get; set; }
        public Decimal? LongImpedimento { get; set; }
        public Decimal? CostoInversion { get; set; }
        public int? PrimerEstrato { get; set; }
        public int? SegundoEstrato { get; set; }
        public int? TercerEstrato { get; set; }
        public int? CuartoEstrato { get; set; }
        public int? QuintoEstrato { get; set; }
        public string? FechaRegistro { get; set; }
        public int? reemplazoId { get; set; }
    }
}
