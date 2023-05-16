using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PerfilResponse
    {
        public int cod_perfil { get; set; }
        public int cod_rol { get; set; }
        public int Perm_viz_modulocodMod_permiso { get; set; }
        public int Permisos_viz_seccioncodSec_permViz { get; set; }
        public string nombre_perfil { get; set; }
        public string estado_perfil { get; set; }
    }
}
