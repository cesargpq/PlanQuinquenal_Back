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
        public string nom_perfil { get; set; }
    }
}
