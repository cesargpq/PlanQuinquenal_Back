using PlanQuinquenal.Core.DTOs.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class FiltersProyectos : PaginacionDTO
    {
        public string? CodigoProyecto { get; set; }
        //falta
        public string? NroExpediente { get; set; }
        //falta
        public int? EstadoGeneral { get; set; }
        //falta
       
        public string? NombreProyecto { get; set; }
        public int? MaterialId { get; set; }
        public int? DistritoId { get; set; }
        public int? TipoProyectoId { get; set; }
        public int? PQuinquenalId { get; set; }
        public string? AñoPq { get; set; }
        public int? PAnualId { get; set; }
        public string? CodigoMalla { get; set; }
        public int? ConstructorId { get; set; }
        public int? IngenieroId { get; set; }
        //Falta
        public string? ProblematicaReal { get; set; }
        public int? UsuarioRegisterId { get; set; }
        public string? FechaGasificacion { get; set; }
        public int? TipoProy { get; set; }

    }
}
