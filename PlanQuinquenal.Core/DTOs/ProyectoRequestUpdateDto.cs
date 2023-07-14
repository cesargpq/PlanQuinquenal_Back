using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class ProyectoRequestUpdateDto
    {
        public string CodigoProyecto { get; set; }
        public string Descripcion { get; set; }
        public int PQuinquenalId { get; set; }
        public string AñosPQ { get; set; }
        public int PlanAnualId { get; set; }
        public int MaterialId { get; set; }
        public int DistritoId { get; set; }

        public int TipoProyectoId { get; set; }
        public int Etapa { get; set; }
        public string CodigoMalla { get; set; }
        public int TipoRegistroId { get; set; }
        public int IngenieroResponsableId { get; set; }
        public int ConstructorId { get; set; }
        public int BaremoId { get; set; }

        public string? FechaGacificacion { get; set; }
        public Decimal LongAprobPa { get; set; }
        public Decimal LongRealHab { get; set; }
        public Decimal LongRealPend { get; set; }
        public Decimal LongProyectos { get; set; }
        public List<int> UsuariosInteresados { get; set; } = null;
        

    }
}
