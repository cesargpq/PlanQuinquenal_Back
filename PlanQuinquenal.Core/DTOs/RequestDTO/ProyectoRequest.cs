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
        public Proyectos proyecto { get; set; }
        public List<UsuariosIntersados_pry> lstUsuaInter_Inicial { get; set; }
        public List<UsuariosIntersados_pry> lstUsuaInter_Final { get; set; }
        public List<Permisos_proyec> lstPermisos_Inicial { get; set; }
        public List<Permisos_proyec> lstPermisos_Final { get; set; }
        public List<Impedimentos> lstImpedimentos_Inicial { get; set; }
        public List<Impedimentos> lstImpedimentos_Final { get; set; }
        public List<Docum_proyecto> lstDocumentos_Inicial { get; set; }
        public List<Docum_proyecto> lstDocumentos_Final { get; set; }
        public List<Informes_actas> lstInformes_Inicial { get; set;}
        public List<Informes_actas> lstInformes_Final { get; set; }
        public List<Comentarios_proyec> lstComentarios_Inicial { get; set; }
        public List<Comentarios_proyec> lstComentarios_Final { get; set; }
    }
}
