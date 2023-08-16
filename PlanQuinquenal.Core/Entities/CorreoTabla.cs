using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class CorreoTabla
    {
       
        public int? id { get; set; }
        public int? idNotif { get; set; }
        public string? codigo { get; set; }
        public string? campoModificado { get; set; }
        public string? valorModificado { get; set; }
        public string? fechaMod { get; set; }
        public string? usuModif { get; set; }
    }
}
