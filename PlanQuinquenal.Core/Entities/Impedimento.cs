using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Impedimento
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public int? ProblematicaRealId { get; set; }
        public Decimal? LongImpedimento { get; set; }
        public int? CausalReemplazoId { get; set; }
        public bool? Resuelto { get; set; }
        public int? PrimerEstrato { get; set; }
        public int? SegundoEstrato { get; set; }
        public int? TercerEstrato { get; set; }
        public int? CuartoEstrato { get; set; }
        public int? QuintoEstrato { get; set; }
        public Decimal? LongitudReemplazo { get; set; }
        public bool? ValidacionCargoPlano { get; set; }
        public bool? ValidacionCargoSustentoRRCC { get; set; }
        public bool? ValidacionCargoSustentoAmbiental { get; set; }
        public bool? ValidacionCargoSustentoArqueologia { get; set; }
        public int? ValidacionLegalId { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime? FechaPresentacionReemplazo { get; set; }
        public int? EvidenciaReemplazo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? fechamodifica { get; set; }
        public int? UsuarioRegisterId { get; set; }
        public int? UsuarioModificaId { get; set; }
        public Decimal? CostoInversion { get; set; }
        public bool estado { get; set; }
    }
}
