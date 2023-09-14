using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class MensualDtoResponse
    {
        public string? FechaGasificacion { get; set; }
        public string CodigoPlan { get; set; }
        public Decimal LongitudConstruida { get; set; }
    }
}
