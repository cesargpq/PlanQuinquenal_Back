using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ListaDocImpedimentosDTO:PaginacionDTO
    {
        public string NombreDocumento { get; set; }
        public int CodigoImpedimento { get; set; }
        public string Gestion { get; set; }
    }
}
