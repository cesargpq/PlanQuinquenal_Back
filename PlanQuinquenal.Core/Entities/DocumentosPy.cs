using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class DocumentosPy
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string CodigoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoDocumento { get; set; }
        public string Ruta { get; set; }
        public bool Estado { get; set; }
        public string rutafisica { get; set; }
        public DateTime Aprobaciones { get; set; }

    }

}
