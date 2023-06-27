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
        Task<PaginacionResponseDto<MaestroResponseDto>> GetAll(ListEntidadDTO entidad);
        Task<MaestroResponseDto> GetById(int id, string maestro);
        Task<IEnumerable<MaestroResponseDto>> GetAllByAttribute(string attribute);
        Task<bool> DeleteById(int id, string maestro);
        Task<bool> Post(PostEntityReqDTO postEntityReqDTO);
        Task<bool> Update(PostUpdateEntityDTO postEntityReqDTO,int id);
    }
}
