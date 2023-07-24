using PlanQuinquenal.Core.DTOs;
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

        Task<ResponseDTO> Add(ComentarioRequestDTO c,DatosUsuario usuario);

        Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPY(RequestComentarioDTO p, int idUser);
        Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPQ(RequestComentarioDTO p, int idUser);
        Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarBR(RequestComentarioDTO p, int idUser);
        Task<PaginacionResponseDtoException<ComentarioResultDTO>> ListarPA(RequestComentarioDTO p, int idUser);
    }
}
