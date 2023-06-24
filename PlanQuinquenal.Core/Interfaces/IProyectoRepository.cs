using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IProyectoRepository
    {
        Task<ProyectoResponseDto> GetById(int id);
        Task<List<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos);
    }
}
