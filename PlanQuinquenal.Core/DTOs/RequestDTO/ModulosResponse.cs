using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ModulosResponse
    {
        public string idMensaje { get; set; }
        public string mensaje { get; set; }
        public TablaPerm_viz_modulo perm_modulos { get; set; }
        public List<TablaPermisos_viz_seccion> perm_campos { get; set; }
    }
}
