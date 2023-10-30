using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class RequestBolsaDto
    {
        public string CodigoProyecto { get; set; }
        public int DistritoId { get; set; }
        public int ContratistaId { get; set; }
        public string CodigoMalla { get; set; }
        public string? Importancia { get; set; }
    }
}
