using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class UsuariosInteresadosPA
    {
        public int Id { get; set; }
        public int PlanAnualId { get; set; }
        public int UsuarioId { get; set; }
        public bool estado { get; set; }
    }
}
