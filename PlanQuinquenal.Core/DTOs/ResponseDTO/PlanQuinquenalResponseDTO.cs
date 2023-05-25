using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class PlanQuinquenalResponseDTO
    {
        public int Id { get; set; }
        public int Pq { get; set; }
        public int EstadoId { get; set; }
        public DateTime Aprobaciones { get; set; }
        public string Descripcion { get; set; }
    }
}
