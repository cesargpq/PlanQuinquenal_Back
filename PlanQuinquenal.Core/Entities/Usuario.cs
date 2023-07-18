using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Usuario
    {
        [Key]
        public int cod_usu { get; set; }
        public string? nombre_usu { get; set; }
        public string? apellido_usu { get; set; }
        public string? correo_usu { get; set; }
        public string? passw_user { get; set; }
        public string? estado_user { get; set; }
        public int? cod_rol { get; set; }
        
        public int? Perfilcod_perfil { get; set; }
        public Perfil? Perfil { get; set; }
        public int? Unidad_negociocod_und { get; set; }
        public Unidad_negocio? Unidad_negocio { get; set; }
        public bool Estado { get; set; }
        public int? Intentos { get; set; }
        public bool Interno { get; set; }
        public DateTime? LastSesion { get; set; }
        public bool Conectado { get; set; }
        public bool DobleFactor { get; set; }
        public DateTime? FechaModifica { get; set; }
        public DateTime? FechaCreacion { get; set; }

        

    }
}
