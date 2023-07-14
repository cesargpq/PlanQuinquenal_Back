using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class RequestComentarioDTO:PaginacionDTO
    {
        public int Codigo { get; set; }
        public int TipoSeguimientoId { get; set; }
    }
}
