using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class PaginacionResponseDto<T>
    {

        public int Cantidad { get; set; }
        public List<T> Model { get; set; }
    }
}
