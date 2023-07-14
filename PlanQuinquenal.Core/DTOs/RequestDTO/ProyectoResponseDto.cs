using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ProyectoResponseDto
    {
        public int Id { get; set; }
        public string CodigoProyecto { get; set; }
        public string descripcion { get; set; }
        public int PQuinquenalId { get; set; }
        public string AñosPQ { get; set; }
        public int? PlanAnualId { get; set; }
        public int MaterialId { get; set; }
        public int DistritoId { get; set; }
        public Material Material { get; set; }
        public Distrito Distrito { get; set; }
        public Constructor Constructor { get; set; }
        public TipoProyecto TipoProyecto { get; set; }
        public TipoRegistro TipoRegistro { get; set; }
        public UsuarioResponseDto IngenieroResponsables { get; set; }
        public PQuinquenalResponseDto PQuinquenalResponseDto { get; set; }
        
        public PlanAnualResponseDto PlanAnualResponseDto { get; set; }
        public List<UsuariosInteresadosPyResponseDto> UsuariosInteresados { get; set; }
        public int TipoProyectoId { get; set; }
        public int Etapa { get; set; }

        public Decimal Avance { get; set; }
        public string CodigoMalla { get; set; }
        public int TipoRegistroId { get; set; }
        public int IngenieroResponsableId { get; set; }
        public int ConstructorId { get; set; }
        public int UsuarioRegisterId { get; set; }
        public string EstadoGeneralDesc { get; set; }
        public int BaremoId { get; set; }

        public Decimal LongAprobPa { get; set; }
        public Decimal LongRealHab { get; set; }
        public Decimal LongRealPend { get; set; }
        public Decimal LongImpedimentos { get; set; }
        public Decimal LongReemplazada { get; set; }
        public Decimal longPendienteEjecución { get; set; }
        public Decimal LongProyectos { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? fechamodifica { get; set; }
        public string? FechaGasificacion { get; set; }
        public Baremo Baremo { get; set; }

       
    }
}
