using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ConfRolesPerm : PaginacionDTO
    {
        public int cod_perm_campo { get; set; }
        public bool visib_campo { get; set; }
        public bool edit_campo { get; set; }
    }
}
