using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PQuinquenalRequestDTO :PaginacionDTO
    {
        public string PQ { get; set; }
        public string Aprobaciones { get; set; }
        public int? EstadoAprobacion { get; set; }
        public string Descripcion { get; set; }

        public string FechaRegistro { get; set; }
        public string FechaModificacion { get; set; }
        public int? UsuarioRegister { get; set; }
        public int? UsuarioModifica { get; set; }
    }
}
