using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Entities
{
    public class Informe
    {
        public int Id { get; set; }
        public string? CodigoProyecto { get; set; }
        public int? Etapa { get; set; }
        public DateTime? FechaInforme { get; set; }
        public string? ResumenGeneral { get; set; }
        public string? RutaFisica { get; set; }
        public string? Ruta { get; set; }
        public string? CodigoExpediente { get; set; }
        public string? ActividadesRealizadas { get; set; }
        public string? ProximosPasos { get; set; }

        //Acta

        public DateTime? FechaReunion { get; set; }
        public string? Agenda { get; set; }
        public string? Objetivos { get; set; }
        public string? Acuerdos { get; set; }
        public string? Compromisos { get; set; }
        public DateTime? FechaCompromiso { get; set; }
        public int? Responsable { get; set; }

        public List<ActaParticipantes> Participantes { get; set; }
        public List<ActaAsistentes> Asistentes { get; set; }
        //Acta

        public int? UsuarioRegister { get; set; }
        public int? UsuarioModifica { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? TipoSeguimiento { get; set; }
        public string? Tipo { get; set; }
        public List<ActaDinamica> ActaDinamica { get; set; }
        public List<UsuariosInteresadosInformes>? UserInteresados { get; set; }
        public bool Activo { get; set; }
        public int TipoInformeId { get; set; }

    }
}
