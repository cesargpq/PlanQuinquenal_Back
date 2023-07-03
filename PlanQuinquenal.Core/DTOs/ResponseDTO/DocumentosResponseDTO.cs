using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class DocumentosResponseDTO
    {
        public int Id { get; set; }
        public string NombreDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string FechaEmision { get; set; }
  
    }
}
