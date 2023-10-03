using PlanQuinquenal.Core.DTOs.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs
{
    public class PermisosListDto : PaginacionDTO
    {
        public int ProyectoId { get; set; }
        public string tipoPermiso { get; set; }

    }
}
