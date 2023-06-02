using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Impedimentos
    {
        [Key]
        public int id { get; set; }
        public int id_pry { get; set; }
        public string cod_pry { get; set; }
        public int cod_pbReal { get; set; }
        public float long_imped { get; set; }
        public int cod_cauReemp { get; set; }
        public float estrato1 { get; set; }
        public float estrato2 { get; set; }
        public float estrato3 { get; set; }
        public float estrato4 { get; set; }
        public float estrato5 { get; set; }
        public float long_reemp { get; set; }
        public float costo_inv { get; set; }
        public bool valid_cargo_planos { get; set; }
        public bool valid_cargo_susAmb { get; set; }
        public bool valid_cargo_susArq { get; set; }
        public bool valid_cargo_susRRCC { get; set; }
        public int cod_validLegal { get; set; }
        public DateTime fecha_prestReemp { get; set; }
        public string coment_eva { get; set; }
    }
}
