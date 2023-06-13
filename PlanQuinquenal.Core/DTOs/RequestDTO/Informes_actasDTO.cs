using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class Informes_actasDTO
    {
        public int id { get; set; }
        public int cod_tipoSeg { get; set; }
        public int id_pry { get; set; }
        public int cod_tipoDoc { get; set; }
        public bool aprobacion { get; set; }
        public DateTime fecha_emis { get; set; }
        public string tipoAccion { get; set; }
    }
}
