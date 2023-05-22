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
        Task<Object> NuevoProyecto(Proyectos nvoProyecto);
        Task<Object> NuevosProyectosMasivo(ProyectoRequest reqMasivo);
        Task<List<Proyectos>> ObtenerProyectos(FiltersProyectos filterProyectos);
        Task<Proyectos> ObtenerProyectoxNro(string nroProy);
    }
}
