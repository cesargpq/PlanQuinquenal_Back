using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ProyectoDetalle
    {
        public int? Id { get; set; }
        public int Total { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? CodigoMalla { get; set; }
        public string? descripcion { get; set; }
        public int? PQuinquenalId { get; set; }
        public string? AnioPlanPQ { get; set; }
        public string? AñosPQ { get; set; }
        public int? PlanAnualId { get; set; }
        public string? AnioPlanPA { get; set; }
        public int? MaterialId { get; set; }
        public string? Material { get; set; }
        public string? ProblematicaReal { get; set; }
        public int? DistritoId { get; set; }
        public string? Distrito { get; set; }
        public int? ConstructorId { get; set; }
        public string? Constructor { get; set; }
        public int? TipoProyectoId { get; set; }
        public string? TipoProyecto { get; set; }
        public int? TipoRegistroId { get; set; }

        public string? TipoRegistro { get; set; }
        public int? IngenieroResponsableId { get; set; }
        public string? IngenieroResponsable { get; set; }
        public Decimal? LongRealPend { get; set; }
        public Decimal? LongAprobPa { get; set; }
        public Decimal? LongReemplazada { get; set; }
        public Decimal? LongConstruida { get; set; }
        public Decimal? LongRealHab { get; set; }
        
        public Decimal? LongImpedimentos { get; set; }
        public Decimal? longPendienteEjecucion { get; set; }
        public Decimal? LongProyectos { get; set; }
        public Decimal? PorcentajeAvance { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? fechamodifica { get; set; }
        public DateTime? FechaGasificacion { get; set; }

        public string? EstadoGeneral { get; set; }
        public Decimal? CostoInversion { get; set; }
        public Decimal? InversionEjecutada { get; set; }

        //Permisos

        public Decimal? LongitudLocal { get; set; }
        public string PermisoLocal { get; set; }
        public Decimal? LongitudMetropolitano { get; set; }
        public string PermisoMetropolitano { get; set; }
        public Decimal? LongitudAmbiental { get; set; }
        public string PermisoAmbiental { get; set; }
        public Decimal? LongitudArqueologia { get; set; }
        public string PermisoArqueologia { get; set; }
        public Decimal? LongitudSociales { get; set; }
        public string PermisoSociales { get; set; }

    }
}
