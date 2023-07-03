using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ActaParticipantes
    {
        public int Id { get; set; }
        public int InformeId { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public bool Activo { get; set; }
    }
}
