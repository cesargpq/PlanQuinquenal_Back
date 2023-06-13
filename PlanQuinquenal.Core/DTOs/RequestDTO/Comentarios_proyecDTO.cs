using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class Comentarios_proyecDTO
    {
        public int id { get; set; }
        public int codigo { get; set; }
        public string comentario { get; set; }
        public string tipo_coment { get; set; }
        public string usuario { get; set; }
        public string area { get; set; }
        public DateTime fecha_coment { get; set; }
        public string tipoAccion { get; set; }
        public int cod_usu { get; set; }
    }
}
