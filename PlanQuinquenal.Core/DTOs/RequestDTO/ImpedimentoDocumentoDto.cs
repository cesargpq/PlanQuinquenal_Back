using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ImpedimentoDocumentoDto
    {
        
        public int CodigoImpedimento { get; set; }
        public string Gestion { get; set; }
        public string base64 { get; set; }
        public string NombreDocumento { get; set; }
        public string? NombreAdicional { get; set; }
    }
}
