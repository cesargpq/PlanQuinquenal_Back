using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class DocumentosPermisosRequestDTO
    {
        public string base64 { get; set; }
        //public string CodigoProyecto { get; set; }
        //public int Etapa { get; set; }
        public int ProyectoId { get; set; }
        public string TipoPermisosProyecto { get; set; }
        public string NombreDocumento { get; set; }
        public string Fecha { get; set; }
        public string Vencimiento { get; set; }
        public string Expediente { get; set; }

    }
}
