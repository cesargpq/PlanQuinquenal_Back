using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class LoginResponseDTO
    {
        public string idMensaje { get; set; }
        public string mensaje { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set;}
        public string correo { get; set; }
        public int cod_perfil { get; set; }
        public int cod_secs_perf { get; set; }
        public int cod_modulo_perf { get; set; }
        //public List<TablaPerm_viz_modulo> perm_modulos { get; set; }
        //public List<TablaPermisos_viz_seccion> perm_campos { get; set; }
    }
}
