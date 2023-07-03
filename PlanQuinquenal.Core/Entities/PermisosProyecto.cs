using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class PermisosProyecto
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public int TipoPermisosProyectoId { get; set; }
        public Decimal Longitud { get; set; }
        public int EstadoPermisosId { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int UsuarioCreacion { get; set; }
        public int UsuarioModifca { get; set; }
       
    }
}
