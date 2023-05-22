using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class UsuarioListDTO : PaginacionDTO
    {
        public int cod_usuario { get; set; }
        public string nombre_usu { get; set; }
        public string apellido_usu { get; set; }
        public string correo_usu { get; set; }
        public string estado { get; set; }
        public int cod_rol { get; set; }
        public int cod_perfil { get; set; }

    }
}
