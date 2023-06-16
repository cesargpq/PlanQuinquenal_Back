using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Permisos_proyec
    {
        [Key]
        [Description("ID del permiso")]
        public int id { get; set; }

        [Description("ID del proyecto")]
        public int id_pry { get; set; }

        [Description("Código del tipo de permiso")]
        public int cod_tipoPerm { get; set; }

        [Description("Nombre del documento")]
        public string nom_doc { get; set; }

        [Description("Número de expediente")]
        public string num_exp { get; set; }

        [Description("Fecha de registro")]
        public DateTime fecha_reg { get; set; }

        [Description("Tipo MIME")]
        public string mime_type { get; set; }
    }
}
