using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ColumsTablaPerResponse
    {
        public string idMensaje { get; set; }
        public string mensaje { get; set; }
        public List<ColumTablaUsu> perm_ColumTabla { get; set; }
    }
}
