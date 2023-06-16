using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class CorreoTablaDTO
    {
        public int idNotif { get; set; }
        public string codigo { get; set; }
        public string campoModificado { get; set; }
        public string valorModificado { get; set; }
        public string fechaMod { get; set; }
        public string usuModif { get; set; }
        
    }
}
