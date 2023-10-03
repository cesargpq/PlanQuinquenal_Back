using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ResposeDistritosDetalleDTO
    {
        public List<string> categorias { get; set; }
        public List<decimal?> norequiere { get; set; }
        public List<decimal?> permisodenegado { get; set; }
        public List<decimal?> permisotramite { get; set; }

        public List<decimal?> permisonotramitado { get; set; }
        public List<decimal?> permisootorgado { get; set; }
        public List<decimal?> sap { get; set; }
    }
}
