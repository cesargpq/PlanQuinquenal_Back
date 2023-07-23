using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ActualizarPermisoRequestDto
    {
        public int codigo { get; set; }
        public bool flag_edit { get; set; }
    }
}
