using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryProyecto
    {
        Task<Object> ActualizarProyecto(ProyectoRequest nvoProyecto, int idUser);
        Task<Object> CrearDocumento(DocumentoProyRequest requestDoc);
        //Task<Object> CrearImpedimento(ImpedimentoRequest impedimento);
        Task<Object> NuevoProyecto(ProyectoRequest nvoProyecto, int idUser);
        Task<Object> NuevosProyectosMasivo(ProyectoRequest reqMasivo);
        Task<List<Proyectos>> ObtenerProyectos(FiltersProyectos filterProyectos);
        Task<Object> ObtenerProyectoxNro(int nroProy);
    }
}
