﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Usuario
    {
        [Key]
        public int cod_usu { get; set; }
        public string nombre_usu { get; set; }
        public string apellido_usu { get; set; }
        public string correo_usu { get; set; }
        public string passw_user { get; set; }
        public string estado_user { get; set; }
        public int cod_rol { get; set; }
        public int Perfilcod_perfil { get; set; }
        public Perfil Perfil { get; set; }
        public int cod_und { get; set; }
    }
}
