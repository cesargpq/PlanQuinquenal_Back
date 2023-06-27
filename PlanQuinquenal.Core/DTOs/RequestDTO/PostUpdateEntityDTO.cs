using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PostUpdateEntityDTO
    {
        public string Descripcion { get; set; }
        public string Entidad { get; set; }
        public bool Estado { get; set; }
    }
}
