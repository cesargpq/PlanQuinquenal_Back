using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
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
        Task<PaginacionResponseDto<TablaLogicaDatos>> GetAll(ListEntidadDTO entidad);
        Task<TablaLogicaDatos> GetById(int id);
        Task<IEnumerable<TablaLogicaDatos>> GetAllByAttribute(string attribute);
        Task<bool> DeleteById(int id);
        Task<bool> Post(PostEntityReqDTO postEntityReqDTO);
        Task<bool> Update(PostUpdateEntityDTO postEntityReqDTO,int id);
    }
}
