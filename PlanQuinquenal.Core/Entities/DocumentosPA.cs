using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class DocumentosPA
    {
        public int Id { get; set; }
        public int PlanAnualId { get; set; }
        public string CodigoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoDocumento { get; set; }
        public string Ruta { get; set; }
        public bool Estado { get; set; }
    }
}
