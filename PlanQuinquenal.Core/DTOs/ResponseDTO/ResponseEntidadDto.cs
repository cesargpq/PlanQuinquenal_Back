using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ResponseEntidadDto<T>
    {
        public T Model { get; set; }
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
