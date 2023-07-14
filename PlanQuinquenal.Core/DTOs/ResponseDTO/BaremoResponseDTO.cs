using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class BaremoResponseDTO
    {
        public int Id { get; set; }
        public string CodigoBaremo { get; set; }
        public string Descripcion { get; set; }
        public Decimal Precio { get; set; }
        public bool Estado { get; set; }
        public string PlanQuinquenalId { get; set; }
        public string PlanQuinquenal { get; set; }
    }
}
