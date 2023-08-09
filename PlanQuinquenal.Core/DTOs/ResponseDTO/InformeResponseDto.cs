using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.ResponseDTO
{
    public class InformeResponseDto
    {
        public int Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public int? Etapa { get; set; }
        public string? FechaInforme { get; set; }
        public string? ResumenGeneral { get; set; }
        public string? RutaFisica { get; set; }
        public string? Ruta { get; set; }
        public string? CodigoExpediente { get; set; }
        public string? ActividadesRealizadas { get; set; }
        public string? ProximosPasos { get; set; }

        //Acta

        public string? FechaReunion { get; set; }
        public string? Agenda { get; set; }
        public string? Objetivos { get; set; }
        public string? Acuerdos { get; set; }
        public string? Compromisos { get; set; }
        public string? FechaCompromiso { get; set; }
        public int? Responsable { get; set; }

 
        public List<UsuariosInteresadosInformesResponseDto> Participantes { get; set; }
        public List<UsuariosInteresadosInformesResponseDto> Asistentes { get; set; }
  
        //Acta
        public List<ActaDinamica> ActaDinamica { get; set; }
        public int? UsuarioRegister { get; set; }
        public int? UsuarioModifica { get; set; }
        public string? FechaCreacion { get; set; }
        public string? FechaModificacion { get; set; }
        public string? TipoSeguimiento { get; set; }
        public int? TipoSeguimientoId { get; set; }
        public string? Tipo { get; set; }
        public int TipoInformeId { get; set; }
        public bool? actaEstado { get; set; }
        public bool actaEstadoUser { get; set; }
        public List<UsuariosInteresadosInformesResponseDto> UserInteresados { get; set; }
    }
}
