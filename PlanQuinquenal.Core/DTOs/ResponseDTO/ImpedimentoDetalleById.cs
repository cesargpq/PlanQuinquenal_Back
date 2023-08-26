using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ImpedimentoDetalleById
    {
        public int Total { get; set; }
        public int Id { get; set; }
        public string? PQ { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? CodigoMalla { get; set; }
        public string? PlanAnualId { get; set; }
        public Decimal? LongImpedimento { get; set; }
        public int? CausalReemplazoId { get; set; }
        public string? CausalReemplazo { get; set; }

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
        public int? ProblematicaRealId { get; set; }
        public bool? ValidacionCargoPlano { get; set; }
        public bool? ValidacionCargoSustentoRRCC { get; set; }
        public bool? ValidacionCargoSustentoAmbiental { get; set; }

        public bool? ValidacionCargoSustentoArqueologia { get; set; }
        public int? ValidacionLegalId { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public string? Comentario { get; set; }
    }
}
