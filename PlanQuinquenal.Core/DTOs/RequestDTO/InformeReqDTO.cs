using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class InformeReqDTO
    {
        public string CodigoProyecto { get; set; }
        public int? Modulo { get; set; }
       
        public int? Tipo { get; set; }
        public string? FechaInforme { get; set; }
        public string? ResumenGeneral { get; set; }
        public string? ActividadesRealizadas { get; set; }
        public string? ProximosPasos { get; set; }
   
        public List<int>? UserInteresados { get; set;}
        public List<int>? UserParticipantes { get; set; }
        public List<int>? UserAsistentes { get; set; }

        //Acta

        public string? FechaReunion { get; set; }
        public string? Agenda { get; set; }
        public string? Objetivos { get; set; }
        public string? Acuerdos { get; set; }
        public string? Compromisos { get; set; }
        public string? FechaCompromiso { get; set; }
        public int? Responsable { get; set; }

        public List<int>? Participantes { get; set; }
        public List<int>? Asistentes { get; set; }
        public List<ActaDinamicaRequest>? ActaDinamicaRequest { get; set; }
    }
}
