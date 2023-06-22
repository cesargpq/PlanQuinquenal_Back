using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Proyectos
    {
        [Key]
        [Description("ID del proyecto")]
        public int id { get; set; }

        [Description("Descripción del proyecto")]
        public string? des_pry { get; set; }

        [Description("Código del proyecto")]
        public string cod_pry { get; set; }

        [Description("Código del PQ")]
        public int cod_PQ { get; set; }

        [Description("Año del PQ")]
        public string anioPQ { get; set; }

        [Description("Código del año PA")]
        public int cod_anioPA { get; set; }

        [Description("Código del material")]
        public int cod_material { get; set; }
       

        [Description("Constructor")]
        public int constructor { get; set; }
        [Description("Estado General")]
        public int EstadoGeneralId { get; set; }

        [Description("Tipo de registro")]
        public int tipo_reg { get; set; }

        [Description("Código del distrito")]
        public int cod_dist { get; set; }

        [Description("Longitud aprobada")]
        public Decimal long_aprob { get; set; }

        [Description("Longitud real pendiente")]
        public Decimal long_realPend { get; set; }

        [Description("Longitud real habilitada")]
        public Decimal long_realHabi { get; set; }

        [Description("Longitud de Impedimento")]
        public Decimal long_impedimento { get; set; }
        [Description("Longitud reemplazada")]
        public Decimal long_reemplazada { get; set; }

        [Description("Longitud de proyectos")]
        public Decimal long_proyectos { get; set; }

        [Description("Tipo de proyecto")]
        public int tipo_pry { get; set; }

        [Description("Código de etapa")]
        public int cod_etapa { get; set; }

        [Description("Código de malla")]
        public string cod_malla { get; set; }

        [Description("Ingeniero responasble")]
        public string ingRespon { get; set; }

        [Description("Usuario de registro")]
        public int user_reg { get; set; }

        [Description("Fecha de gasificacion")]
        public DateTime fecha_gas { get; set; }

        [Description("Código de VNR")]
        public int cod_vnr { get; set; }
    }
}
