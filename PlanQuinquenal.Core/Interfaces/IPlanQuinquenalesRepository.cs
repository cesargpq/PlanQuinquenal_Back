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
    public interface IPlanQuinquenalesRepository
    {
        Task<IEnumerable<PlanQuinquenal.Core.Entities.PlanQuinquenal>> Get();
        Task<PaginacionResponseDtoException<PQuinquenalResponseDto>> GetAll(PQuinquenalRequestDTO p);

        Task<PaginacionResponseDtoException<PQuinquenalResponseDto>> GetSeleccionados(PlanQuinquenalSelectedId p);

        Task<ResponseDTO> Add(PQuinquenalReqDTO p,DatosUsuario usuario);
        Task<ResponseEntidadDto<PQuinquenalResponseDto>> GetById(int id);

        Task<PaginacionResponseDtoException<AvancePQDto>> GetAvance(int id);
        Task<ResponseDTO> Update(UpdatePlanQuinquenalDto dto, int id, DatosUsuario usuario);
        //Task<bool> CreatePQ(PQuinquenalReqDTO pQuinquenalReqDTO, int idUser);
        //Task<Object> ActualizarPQ(PQuinquenalReqDTO planquinquenal, int idUser);
        Task<Object> CrearComentario(Comentarios_proyecDTO comentario, int idUser, string modulo);
        Task<Object> EliminarComentario(int codigo, string modulo);
        Task<Object> CrearDocumento(Docum_proyectoDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarDocumento(int codigo, string modulo);
        Task<Object> CrearInforme(InformeRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> ModificarInforme(InformeRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarInforme(int codigo, string modulo);
        Task<Object> CrearActa(ActaRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> ModificarActa(ActaRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarActa(int codigo, string modulo);
    }
}
