using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Informes_actas
    {
        [Key]
        public string id { get; set; }
        public string cod_tipoSeg { get; set; }
        public string des_tipoSeg { get; set; }
        public int cod_seg { get; set; }
        public string des_seg { get; set; }
        public string cod_tipoDoc { get; set; }
        public bool aprobacion { get; set; }
        public DateTime fecha_emis { get; set; }
    }
}
