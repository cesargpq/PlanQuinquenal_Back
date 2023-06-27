using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class DocumentoResponseDto
    {
        public string ruta { get; set; }
        public string nombreArchivo { get; set; }
        public string tipoDocumento { get; set; }
    }
}
