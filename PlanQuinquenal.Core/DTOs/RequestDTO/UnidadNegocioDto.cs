using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class UnidadNegocioDto : PaginacionDTO
    {
        public int cod_und { get; set; }
        public string nom_und { get; set; }
        public string estado_und { get; set; }
        public string buscador { get; set; }
    }
}
