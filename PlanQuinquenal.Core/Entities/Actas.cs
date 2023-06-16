using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Actas
    {
        [Key]
        [Description("ID del acta")]
        public int id { get; set; }

        [Description("Fecha de la reunión")]
        public DateTime fecha_reu { get; set; }

        [Description("Agenda de la reunión")]
        public string agenda { get; set; }

        [Description("Acuerdos de la reunión")]
        public string acuerdos { get; set; }

        [Description("Objetivo de la reunión")]
        public string obj_reu { get; set; }

        [Description("Compromisos de la reunión")]
        public string compromisos { get; set; }

        [Description("Fecha límite para los compromisos")]
        public DateTime fecha_limComp { get; set; }

        [Description("Código del responsable")]
        public int cod_resp { get; set; }

        [Description("Fecha de emisión del acta")]
        public DateTime fecha_emis { get; set; }

        [Description("Aprobación del acta")]
        public bool aprobacion { get; set; }

        [Description("Código del tipo de seguimiento")]
        public int cod_tipoSeg { get; set; }

        [Description("Código del seguimiento")]
        public int cod_seg { get; set; }
    }
}
