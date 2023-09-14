using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class MensualidadDTO
    {
        public List<string> categorias { get; set; } = new List<string>();
        public List<Serie> ListaPqMensual { get; set; }
    }
   
}
