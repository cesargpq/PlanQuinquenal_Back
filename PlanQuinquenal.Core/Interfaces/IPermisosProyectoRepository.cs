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
    public interface IPermisosProyectoRepository
    {
        Task<ResponseDTO> Add(PermisoRequestDTO permisoRequestDTO, int idUser);
        Task<ResponseEntidadDto<PermisoByIdResponseDto>> GetPermiso(int idProyecto, string TipoPermiso);
        Task<ResponseDTO> CargarExpediente(DocumentosPermisosRequestDTO documentosPermisosRequestDTO,int idUser);
        Task<ResponseDTO> Delete(int id, int idUser);
        Task<PaginacionResponseDto<DocumentoPermisosResponseDTO>> Listar(ListDocumentosRequestDto listDocumentosRequestDto);
        Task<ResponseEntidadDto<DocumentoPermisosResponseDTO>> Download(int id);

    }
}
