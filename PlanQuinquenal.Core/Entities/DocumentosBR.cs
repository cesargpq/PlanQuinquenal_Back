using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class DocumentosBR
    {
        public int Id { get; set; }
        public int? BolsaReemplazoId { get; set; }
        public string? CodigoDocumento { get; set; }
        public string? NombreDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string? TipoDocumento { get; set; }
        public string? ruta { get; set; }
        public bool? Estado { get; set; }
        public string? rutaFisica { get; set; }
        public DateTime? Aprobaciones { get; set; }
    }
}
