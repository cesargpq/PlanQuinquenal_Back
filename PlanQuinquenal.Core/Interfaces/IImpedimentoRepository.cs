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
    public interface IImpedimentoRepository
    {
        Task<ResponseDTO> Add(ImpedimentoRequestDTO p, int idUser);
        Task<ResponseDTO> Update(ImpedimentoUpdateDto p, int idUser,int id);
        Task<ResponseEntidadDto<ImpedimentoDetalleById>> GetById(int Id);
        Task<DocumentoResponseDto> Download(int id);
        Task<PaginacionResponseDtoException<ImpedimentoDetalle>> Listar(ImpedimentoListDto p);

        Task<ResponseDTO> Documentos(ImpedimentoDocumentoDto p,int idUser);

        Task<PaginacionResponseDtoException<Object>> ListarDocumentos(ListaDocImpedimentosDTO p);
        Task<ImportResponseDto<Impedimento>> ProyectoImport(RequestMasivo data);
        Task<ResponseDTO> Delete(int id, int idUser);
        Task<ResponseDTO> DeleteDocumentos(int id, int idUser);
    }
}
