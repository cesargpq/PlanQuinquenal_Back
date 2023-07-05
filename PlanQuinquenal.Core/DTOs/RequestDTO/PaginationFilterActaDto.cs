using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PaginationFilterActaDto:PaginacionDTO
    {
        public int TipoDocumento { get; set; }
        public string CodigoDocumento { get; set; }
        public string CodigoProyecto { get; set; }
        public int Etapa { get; set; }
        public int TipoSeguimiento { get; set; }
    }
}
