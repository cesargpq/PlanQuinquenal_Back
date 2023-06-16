using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ImportResponseDto<T> : ResponseDTO
    {
        public List<T> listaError { get; set; }
        public List<T> listaRepetidos { get; set; }
        public List<T> listaInsert { get; set; }
        public int Satisfactorios { get; set; }
        public int Error { get; set; }
        public int Actualizados { get; set; }

    }
}
