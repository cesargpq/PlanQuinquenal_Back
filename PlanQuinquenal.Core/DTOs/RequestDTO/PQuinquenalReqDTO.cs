using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PQuinquenalReqDTO
    {
        public string AnioPlan { get; set; }
        public int EstadoAprobacionId { get; set; }
        public string FechaAprobacion { get; set; }
        public string Descripcion { get; set; }
        public List<int> UsuariosInteresados { get; set; }
    }
}
