using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ListDocumentosRequestDto: PaginacionDTO
    {
        public string Modulo { get; set; }
        public string CodigoProyecto { get; set; }
        public string Buscar { get; set; } 
    }
}
