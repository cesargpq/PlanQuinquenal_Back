using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class FiltersProyectos
    {
        public string des_pry { get; set; }
        public string cod_pry { get; set; }
        public int nro_exp { get; set; }
        public string estado_pry { get; set; }
        public int cod_etapa { get; set; }
        public int por_avance { get; set; }
        public int cod_material { get; set; }
        public int cod_dist { get; set; }
        public int tipo_pry { get; set; }
        public int cod_PQ { get; set; }
        public string anioPQ { get; set; }
        public int cod_anioPA { get; set; }
        public string cod_malla { get; set; }
        public string constructor { get; set; }
        public string ingRespon { get; set; }
        public string user_reg { get; set; }
        public string fecha_gas { get; set; }
        public int cod_pryReemp { get; set; }
    }
}
