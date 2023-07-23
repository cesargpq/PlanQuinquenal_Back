using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ColumTablaUsuResponseDto
    {
        public int id { get; set; }
        public bool seleccion { get; set; }
        public string title { get; set; }
        public string campo { get; set; }
        public string tipo { get; set; }
    }
}
