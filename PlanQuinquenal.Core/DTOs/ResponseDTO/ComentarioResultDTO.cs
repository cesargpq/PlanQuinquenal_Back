using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ComentarioResultDTO
    {
        public string usuario { get; set; }
        public string perfil { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
