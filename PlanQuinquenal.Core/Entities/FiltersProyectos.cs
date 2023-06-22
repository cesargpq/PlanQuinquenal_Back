using PlanQuinquenal.Core.DTOs.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class FiltersProyectos : PaginacionDTO
    {
        public string des_pry { get; set; }
        public string cod_pry { get; set; }
        public string nro_exp { get; set; }
        public int estado_pry { get; set; }
        public int cod_etapa { get; set; }
        public int por_avance { get; set; }
        public int cod_material { get; set; }
        public int cod_dist { get; set; }
        public int tipo_pry { get; set; }
        public int cod_PQ { get; set; }
        public string anioPQ { get; set; }
        public int cod_anioPA { get; set; }
        public string cod_malla { get; set; }
        public int constructor { get; set; }
        public string ingRespon { get; set; }
        public int user_reg { get; set; }
        public string fecha_gas { get; set; }
        public int problematica_real { get; set; }
        public int usuario_reg { get; set; }
    }
}
