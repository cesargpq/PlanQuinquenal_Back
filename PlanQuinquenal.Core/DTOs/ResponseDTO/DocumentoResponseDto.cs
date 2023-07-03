﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class DocumentoResponseDto
    {
        public int Id { get; set; }
        public string ruta { get; set; }
        public string codigoDocumento { get; set; }
        public string nombreArchivo { get; set; }
        public string tipoDocumento { get; set; }
        public string Aprobaciones { get; set; }
    }
}
