using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class UsuariosInteresadosPQ
    {
        public int Id { get; set; }
        public int PQuinquenalId { get; set; }
        public int UsuarioId { get; set; }
        public bool Estado { get; set; }
    }
}
