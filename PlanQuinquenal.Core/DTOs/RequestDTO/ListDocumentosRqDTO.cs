using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ListDocumentosRqDTO:PaginacionDTO
    {
        public string Modulo { get; set; }
        public string codigoProyecto { get; set; }
        public int etapa { get; set; }
        public string Buscar { get; set; }

    }
}
