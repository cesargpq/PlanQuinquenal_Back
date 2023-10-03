using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class PaginacionResponseDtoException<T>
    {
        public int? Cantidad { get; set; }
        public IEnumerable<T> Model { get; set; }
    }
}
