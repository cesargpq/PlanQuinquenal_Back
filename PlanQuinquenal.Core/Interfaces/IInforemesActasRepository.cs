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
    public interface IInforemesActasRepository
    {
        Task<ResponseDTO> Crear(InformeReqDTO informeReqDTO,DatosUsuario usuario);

        Task<ResponseDTO> Update(InformeReqDTO informeReqDTO, int id, DatosUsuario usuario);
        Task<ResponseEntidadDto<InformeResponseDto>> GetById(int id);
        Task<PaginacionResponseDto<InformeResponseDto>> GetAll(PaginationFilterActaDto pag);
        Task<DocumentoResponseDto> Download(int id);

        Task<ResponseDTO> Delete(int id,DatosUsuario usuario);
        Task<ResponseDTO> AprobarActa(AprobarActaDto a, DatosUsuario usuario);
    }
}
