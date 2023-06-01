using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Proyectos
    {
        [Key]
        public int id { get; set; }
        public string des_pry { get; set; }
        public string cod_pry { get; set; }
        public int cod_PQ { get; set; }
        public string anioPQ { get; set; }
        public int cod_anioPA { get; set; }
        public int cod_material { get; set; }
        public string constructor { get; set; }
        public string tipo_reg { get; set; }
        public int cod_dist { get; set; }
        public float long_aprob { get; set; }
        public float long_realPend { get; set; }
        public float long_realHabi { get; set; }
        public float long_proyectos { get; set; }
        public int tipo_pry { get; set; }
        public int cod_etapa { get; set; }
        public string cod_malla { get; set; }
        public string ingRespon { get; set; }
        public string user_reg { get; set; }
        public DateTime fecha_gas { get; set; }
        public int cod_pryReemp { get; set; }
        public string cod_vnr { get; set; }
    }
}
