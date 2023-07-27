using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ProyectoResponseIdDTO
    {
        public int? Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? descripcion { get; set; }
        public int? PQuinquenalId { get; set; }
        public string? AnioPlanPQ { get; set; }
        public string? AñosPQ { get; set; }
        public string? CodigoMalla { get; set; }
        public int? PlanAnualId { get; set; }
        public string? AñoPA { get; set; }
        public string? ProblematicaRealId { get; set; }
        public int? MaterialId { get; set; }
        public string? Material { get; set; }
        public int? DistritoId { get; set; }
        public string? Distrito { get; set; }
        public int? ConstructorId { get; set; }
        public string? Constructor { get; set; }
        public int? TipoProyectoId { get; set; }
        public string? TipoProyecto { get; set; }
        public int? TipoRegistroId { get; set; }
        public string?    TipoRegistro { get; set; }
        public int? IngenieroResponsableId { get; set; }
        public string? IngenieroResponsable { get; set; }
        public Decimal? LongAprobPa { get; set; }
        public Decimal? LongRealPend { get; set; }

        public Decimal? LongImpedimentos { get; set; }
        public Decimal? LongRealHab { get; set; }
        public Decimal? LongReemplazada { get; set; }
        public Decimal? longPendienteEjecucion { get; set; }
        public Decimal? LongProyectos { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? fechamodifica { get; set; }
        public DateTime? FechaGasificacion { get; set; }
        public string? CodigoBaremo { get; set; }
        public int? BaremoId { get; set; }
        public string? EstadoGeneral { get; set; }
        public Decimal? Avance { get; set; }
        public Decimal? CostoInversion { get; set; }
        [NotMapped]
        public List<UsuarioResponseDto> UsuariosInteresados { get; set; }

    }
}
