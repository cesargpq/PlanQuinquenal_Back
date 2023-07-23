using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class DocumentoRequestDto
    {
        public string Modulo { get;set; }
        //public string CodigoProyecto { get; set; }
        public int ProyectoId { get; set; }
        public string base64 { get; set; }
        public string Aprobaciones { get; set; }
        public string NombreDocumento { get; set; }
        public string TipoDocumento { get; set; }
    }
}
