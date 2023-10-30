using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class BolsaDetalleById
    {
        public int Total { get; set; }
        public int? Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? Distrito { get; set; }
        public int? DistritoId { get; set; }
        public int? ConstructorId { get; set; }
        public int? PermisoId { get; set; }
        public int? ReemplazoId { get; set; }
        public string? Constructor { get; set; }
        public string? CodigoMalla { get; set; }
        public string? Permiso { get; set; }
        public int? Estrato1 { get; set; }
        public int? Estrato2 { get; set; }
        public int? Estrato3 { get; set; }
        public int? Estrato4 { get; set; }
        public int? Estrato5 { get; set; }
        public int? EstratoTotal { get; set; }
        public Decimal? CostoInversion { get; set; }
        public Decimal? LongitudReemplazo { get; set; }
        public string? RiesgoSocial { get; set; }
        public string? Reemplazo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModifica { get; set; }
        public string? UsuarioRegister { get; set; }
        public string? UsuarioModifica { get; set; }
        public string? Importancia { get; set; }
    }
}
