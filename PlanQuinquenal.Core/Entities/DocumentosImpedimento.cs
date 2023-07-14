using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class DocumentosImpedimento
    {
        public int Id { get; set; }
        public int ImpedimentoId { get; set; }
        public string Gestion { get; set; }
        public string CodigoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public  string TipoDocumento { get; set; }
        public string ruta { get; set; }
        public string rutaFisica { get; set; }
        public bool Estado { get; set; }
        public int UsuarioRegister { get; set; }
    }
}
