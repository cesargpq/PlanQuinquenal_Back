using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class UsuarioRequestDto
    {
        public string? nombre_usu { get; set; }
        public string? apellido_usu { get; set; }
        public string? correo_usu { get; set; }
        public string? passw_user { get; set; }   
        public int cod_rol { get; set; }
        public int Perfilcod_perfil { get; set; }
        public bool Interno { get; set; }
        public bool DobleFactor { get; set; } = true;
        public string Estado { get; set; }
    }
}
