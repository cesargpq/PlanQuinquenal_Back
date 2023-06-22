using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PQDocumentos
    {
        [Key]
        [Description("ID")]
        public int Id { get; set; }

        [Description("Plan Quinquenal ID")]
        public int PlanQuinquenalId { get; set; }

        [Description("Código de Documento")]
        public string CodDocumento { get; set; }

        [Description("Nombre")]
        public string Nombre { get; set; }

        [Description("Tipo de Documento")]
        public string TipoDocumento { get; set; }

        [Description("Fecha")]
        public DateTime Fecha { get; set; }

        [Description("Estado")]
        public bool estado { get; set; }
    }
}
