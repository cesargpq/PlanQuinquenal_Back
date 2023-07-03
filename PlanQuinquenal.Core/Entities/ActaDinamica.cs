using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class ActaDinamica
    {
        public int Id { get; set; }
        public int InformeId { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
    }
}
