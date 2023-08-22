using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class PQuinquenalResponseDto
    {
        public int? Id { get; set; }
        public string? AnioPlan { get; set; }
        public int? EstadoAprobacionId { get; set; }
        public string? EstadoAprobacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModifica { get; set; }
        public decimal? Avance { get; set; }
        public string? Estado { get; set; }
        public string? UsuarioModifica { get; set; }
        public string? UsuarioRegister { get; set; }
        public int? UsuarioModificaId { get; set; }
        public int? UsuarioRegisterId { get; set; }
        public Decimal? LongHabilitada { get; set; }
        public Decimal? LongPendiente { get; set; }
        public Decimal? InversionEjecutada { get; set; }
        public int total { get; set; }

    }
}
