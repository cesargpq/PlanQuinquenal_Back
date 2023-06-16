using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PerfilListDto: PaginacionDTO
    {
        public string buscador { get; set; }
        public int UnidadNegocioId { get; set; }
    }
}
