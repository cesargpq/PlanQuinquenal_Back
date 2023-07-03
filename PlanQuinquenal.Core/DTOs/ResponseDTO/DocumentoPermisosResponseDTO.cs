using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class DocumentoPermisosResponseDTO
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string ruta { get; set; }
        public string NombreDocumento { get; set; }
        public string CodigoDocumento { get; set; }
        public string Expediente { get; set; }
    }
}
