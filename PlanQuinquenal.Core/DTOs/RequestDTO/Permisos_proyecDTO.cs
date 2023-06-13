using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class Permisos_proyecDTO
    {
        public int id { get; set; }
        public int codigo { get; set; }
        public int cod_tipoPerm { get; set; }
        public string nom_doc { get; set; }
        public string num_exp { get; set; }
        public DateTime fecha_reg { get; set; }
        public string mime_type { get; set; }
        public string tipoAccion { get; set; }
    }
}
