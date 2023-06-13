using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class DocumentoProyRequest
    {
        public string Base64 { get; set; }
        public string NombreArchivo { get; set; }
    }
}
