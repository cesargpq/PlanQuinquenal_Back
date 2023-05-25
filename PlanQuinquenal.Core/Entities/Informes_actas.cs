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
        public int id { get; set; }
        public int cod_tipoSeg { get; set; }
        public int id_pry { get; set; }
        public int cod_tipoDoc { get; set; }
        public bool aprobacion { get; set; }
        public DateTime fecha_emis { get; set; }
    }
}
