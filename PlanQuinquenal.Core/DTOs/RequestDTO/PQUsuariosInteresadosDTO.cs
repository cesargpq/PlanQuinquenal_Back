using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class PQUsuariosInteresadosDTO
    {
        public int Id { get; set; }
        public int PlanQuinquenalId { get; set; }
        public int UsuarioId { get; set; }
        public bool Estado { get; set; }
        public string TipoAccion { get; set; }
    }
}
