﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class InformeRequestDTO
    {
        public int id { get; set; }
        public DateTime fecha_inf { get; set; }
        public string res_general { get; set; }
        public string prox_pasos { get; set; }
        public string act_realiz { get; set; }
        public DateTime fecha_emis { get; set; }
        public int cod_tipoSeg { get; set; }
        public int cod_seg { get; set; }
        public List<UsuariosIntersados_InfDTO> lstUsuInterInformes { get; set; }
    }
}
