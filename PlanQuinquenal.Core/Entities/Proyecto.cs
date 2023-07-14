using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? descripcion { get; set; }
        public int? PQuinquenalId { get; set; }
        public string? AñosPQ { get; set; }
        public int? PlanAnualId { get; set; }
        public int? MaterialId { get; set; }
        public int? DistritoId { get; set; }
        public PQuinquenal PQuinquenal { get; set; }
        public PlanAnual? PlanAnual { get; set; }
        public Material Material { get; set; }
        public Distrito Distrito { get; set; }
        public Constructor? Constructor { get; set; }
        public TipoProyecto? TipoProyecto { get; set; }
        public TipoRegistro? TipoRegistro { get; set; }
        public EstadoGeneral EstadoGeneral { get; set; }
        public Usuario? IngenieroResponsable { get; set; }
        public List<UsuariosInteresadosPy> UsuariosInteresados { get; set;}
        public int? TipoProyectoId { get; set; }
        public int? Etapa { get; set; }

        public string? CodigoMalla { get; set; }
        public int? TipoRegistroId { get; set; }
        public int? IngenieroResponsableId { get; set; }
        public int? ConstructorId { get; set; }
        public int? EstadoGeneralId { get; set; }
        public int? BaremoId { get; set; }
        public int? UsuarioRegisterId { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? fechamodifica { get; set; }
        public DateTime? FechaGasificacion { get; set; }
        public Decimal? LongAprobPa { get; set; }
        public Decimal? LongRealHab { get; set; }
        public Decimal? LongRealPend { get; set; }
        public Decimal? LongImpedimentos { get; set; }
        public Decimal? LongReemplazada { get; set; }
        public Decimal? LongProyectos { get; set; }
        public Baremo? Baremo { get; set; }

        //Impedimentos unificado

    }
}
