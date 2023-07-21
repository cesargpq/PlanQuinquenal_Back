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
    public interface IDocumentosRepository
    {
        Task<ResponseDTO> GetUrl(int id, string modulo);
        Task<DocumentoResponseDto> Download(int id, string modulo);
        Task<ResponseDTO> Add(DocumentoRequestDto documentoRequestDto, int idUser);

        Task<PaginacionResponseDto<DocumentoResponseDto>> Listar(ListDocumentosRqDTO listDocumentosRequestDto);
        Task<ResponseDTO> Delete(int id, string modulo, int idUser);
    }
}
