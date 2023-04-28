using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryMantenedores
    {
        Task<IEnumerable<TablaLogicaDatos>> GetAll(string entidad);
        Task<bool> Post(PostEntityReqDTO postEntityReqDTO); 
    }
}
