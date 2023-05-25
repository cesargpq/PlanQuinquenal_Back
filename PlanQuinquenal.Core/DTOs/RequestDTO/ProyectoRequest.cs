using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.DTOs.RequestDTO
{
    public class ProyectoRequest
    {
        public string base64 { get; set; }
        List<Permisos_proyec> lstPermisos_Inicial { get; set; }
        List<Permisos_proyec> lstPermisos_Final { get; set; }
        List<Impedimentos> lstImpedimentos_Inicial { get; set; }
        List<Impedimentos> lstImpedimentos_Final { get; set; }
        List<Docum_proyecto> lstDocumentos_Inicial { get; set; }
        List<Docum_proyecto> lstDocumentos_Final { get; set; }
        List<Informes_actas> lstInformes_Inicial { get; set;}
        List<Informes_actas> lstInformes_Final { get; set; }
        List<Comentarios_proyec> lstComentarios_Inicial { get; set; }
        List<Comentarios_proyec> lstComentarios_Final { get; set; }
    }
}
