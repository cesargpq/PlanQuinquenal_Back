using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ActaRequestDTO
    {
        public int id { get; set; }
        public DateTime fecha_reu { get; set; }
        public string agenda { get; set; }
        public string acuerdos { get; set; }
        public string obj_reu { get; set; }
        public string compromisos { get; set; }
        public DateTime fecha_limComp { get; set; }
        public int cod_resp { get; set; }
        public DateTime fecha_emis { get; set; }
        public bool aprobacion { get; set; }
        public int cod_tipoSeg { get; set; }
        public int cod_seg { get; set; }
        public List<UsuAsisten_ActaDTO> lstAsistActas { get; set; }
        public List<UsuParticip_ActaDTO> lstPartActas { get; set; }
        public List<CamposDinam_ActaDTO> lstCamposDinamic { get; set; }
    }
}
