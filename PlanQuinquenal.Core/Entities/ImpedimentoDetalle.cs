using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ImpedimentoDetalle
    {
        public int Total { get; set; }
        public int Id { get; set; }
        public string? PQ { get; set; }
        public string? CodigoProyecto { get; set; }
        public int? Etapa { get; set; }
        public string? CodigoMalla { get; set; }
        public Decimal? LongImpedimento { get; set; }
        public int? CausalReemplazoId { get; set; }
        public string? Distrito { get; set; }
        public string? Constructor { get; set; }
        public string? IngenieroResponsable { get; set; }
        public string? ProblematicaReal { get; set; }
        public Decimal? LongitudReemplazo { get; set; }
        public Decimal? CostoInversion { get; set; }
       
        public int? PrimerEstrato { get; set; }
        public int? SegundoEstrato { get; set; }
        public int? TercerEstrato { get; set; }
        public int? CuartoEstrato { get; set; }
        public int? QuintoEstrato { get; set; }
        public int? TotalPotencial { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
