using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
	public class DocumentosPermisos
	{
		public int Id { get; set; }
        public int ProyectoId { get; set; }
        public int TipoPermisosProyectoId { get; set; }
        public string NombreDocumento { get; set; }
        public string CodigoDocumento { get; set; }
        public DateTime? Fecha { get; set; }
        public string Expediente { get; set; }
        public string ruta { get; set; }

        public string rutaFisica { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int UsuarioCreacion { get; set; }
        public int UsuarioModifca { get; set; }
        public bool Estado { get; set; }

        public DateTime? Vencimiento { get; set; }

    }
}
