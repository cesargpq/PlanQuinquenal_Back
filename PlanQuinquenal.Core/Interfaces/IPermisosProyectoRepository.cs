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
    public interface IPermisosProyectoRepository
    {
        Task<ResponseDTO> Add(PermisoRequestDTO permisoRequestDTO, DatosUsuario usuario);
        Task<ResponseEntidadDto<PermisoByIdResponseDto>> GetPermiso(string CodigoProyecto, string TipoPermiso);
        Task<ResponseDTO> CargarExpediente(DocumentosPermisosRequestDTO documentosPermisosRequestDTO,DatosUsuario usuario);
        Task<ResponseDTO> Delete(int id, DatosUsuario usuario);
        Task<PaginacionResponseDto<DocumentoPermisosResponseDTO>> Listar(ListDocumentosRequestDto listDocumentosRequestDto);
        Task<ResponseEntidadDto<DocumentoPermisosResponseDTO>> Download(int id);

    }
}
