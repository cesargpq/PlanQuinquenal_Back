using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ListaPqMensual
    {
        public PlanQuinquenal PlanQuinquenal { get; set; }
        public HabilitadoDto Habilitado { get; set; } = new HabilitadoDto();
    }
}
