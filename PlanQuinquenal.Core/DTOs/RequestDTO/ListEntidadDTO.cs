using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ListEntidadDTO: PaginacionDTO
    {
        public string Entidad { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
    }
}
