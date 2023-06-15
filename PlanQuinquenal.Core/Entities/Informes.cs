using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Informes
    {
        [Key]
        [Description("ID del informe")]
        public int id { get; set; }

        [Description("Fecha del informe")]
        public DateTime fecha_inf { get; set; }

        [Description("Resumen general")]
        public string res_general { get; set; }

        [Description("Próximos pasos")]
        public string prox_pasos { get; set; }

        [Description("Actividades realizadas")]
        public string act_realiz { get; set; }

        [Description("Fecha de emisión")]
        public DateTime fecha_emis { get; set; }

        [Description("Código del tipo de seguimiento")]
        public int cod_tipoSeg { get; set; }

        [Description("Código del seguimiento")]
        public int cod_seg { get; set; }
    }
}
