using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Config_notificaciones
    {
        [Key]
        public int id { get; set; }
        public int cod_usu { get; set; }
        public bool regPQ { get; set; }
        public bool modPQ { get; set; }
        public bool regPry { get; set; }
        public bool modPry { get; set; }
        public bool regImp { get; set; }
        public bool modImp { get; set; }
        public bool regPer { get; set; }
        public bool modPer { get; set; }
        public bool regEviReemp { get; set; }
        public bool modEviReemp { get; set; }
        public bool regCom { get; set; }
        public bool modCom { get; set; }
        public bool regInfActas { get; set; }
        public bool modInfActas { get; set; }
    }
}
