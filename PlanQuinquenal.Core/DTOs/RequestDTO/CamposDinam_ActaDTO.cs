using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class CamposDinam_ActaDTO
    {
        public int id { get; set; }
        public int id_acta { get; set; }
        public string nom_campo { get; set; }
        public string valor_campo { get; set; }
        public string tipoAccion { get; set; }
    }
}
