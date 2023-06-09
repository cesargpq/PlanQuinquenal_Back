using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class ImportResponseDto : ResponseDTO
    {
        public List<Baremo> listaBaremoError { get; set; }
        public List<Baremo> listaBaremoRepetidos { get; set; }
        public List<Baremo> listaBaremoInsert { get; set; }
        public int Satisfactorios { get; set; }
        public int Error { get; set; }
        public int Actualizados { get; set; }

    }
}
