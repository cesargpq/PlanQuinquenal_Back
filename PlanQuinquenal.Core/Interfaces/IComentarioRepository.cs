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
    public interface IComentarioRepository
    {

        Task<ResponseDTO> Add(ComentarioRequestDTO c,int idUser);

        Task<PaginacionResponseDtoException<ComentarioPY>> ListarPY(RequestComentarioDTO p, int idUser);
        Task<PaginacionResponseDtoException<COMENTARIOPQ>> ListarPQ(RequestComentarioDTO p, int idUser);
        Task<PaginacionResponseDtoException<COMENTARIOPA>> ListarPA(RequestComentarioDTO p, int idUser);
    }
}
