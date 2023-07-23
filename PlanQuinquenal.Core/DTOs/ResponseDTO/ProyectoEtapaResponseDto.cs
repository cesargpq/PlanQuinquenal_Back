using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ProyectoEtapaResponseDto
    {
        public string CodigoProyecto { get; set; }
        public string Denominacion { get; set; }
        public int Etapa { get; set; }
        public Decimal LongitudRedes { get; set; }
        public string Constructor { get; set; }
        
        public DateTime FechaGasificacion { get; set; }
        public string IngenieroProyecto { get; set; }
    }
}
