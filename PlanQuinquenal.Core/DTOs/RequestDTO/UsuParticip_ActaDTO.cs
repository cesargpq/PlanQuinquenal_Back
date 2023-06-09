using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class UsuParticip_ActaDTO
    {
        public int id { get; set; }
        public int id_acta { get; set; }
        public int cod_usu { get; set; }
        public string tipoAccion { get; set; }
    }
}
