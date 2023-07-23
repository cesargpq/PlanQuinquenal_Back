using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class MensualDtoResponse
    {
        public string Mes { get; set; }
        public int pquinquenalId { get; set; }
        public Decimal LongitudConstruida { get; set; }
    }
}
