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
        public List<int> norequiere { get; set; }
        public List<int> permisodenegado { get; set; }
        public List<int> permisotramite { get; set; }

        public List<int> permisonotramitado { get; set; }
        public List<int> permisootorgado { get; set; }
        public List<int> sap { get; set; }
    }
}
