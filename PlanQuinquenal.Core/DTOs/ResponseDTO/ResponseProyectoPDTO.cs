using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ResponseProyectoPDTO
    {
        public int Id { get; set; }
        public string? des_pry { get; set; }
        public string cod_pry { get; set; }
        public int cod_PQ { get; set; }
        public string pq_desc { get; set; }
        public string anioPQ { get; set; }
        public int cod_anioPA { get; set; }

        public int cod_material { get; set; }
        public string material_desc { get; set; }
        public int constructor { get; set; }
        public string constructor_desc { get; set; }
        public int tipo_reg { get; set; }
        public string tipo_reg_desc { get; set; }
        public int cod_dist { get; set; }
        public string cod_dist_desc { get; set; }
        public Decimal long_aprob { get; set; }
        public Decimal long_realPend { get; set; }
        public Decimal long_realHabi { get; set; }
        public Decimal long_impedimento { get; set; }
        public Decimal long_reemplazada { get; set; }
        public Decimal long_proyectos { get; set; }
        public int tipo_pry { get; set; }
        public string tipo_pry_desc { get; set; }
        public int cod_etapa { get; set; }
        public string cod_malla { get; set; }
        public string ingRespon { get; set; }
        public int user_reg { get; set; }
        public string user_reg_desc { get; set; }
        public DateTime fecha_gas { get; set; }
        public int cod_vnr { get; set; }
        public string cod_vnr_desc { get; set; }
        public int EstadoGeneralId { get; set; }
        public string estado_general { get; set; }

    }
}
