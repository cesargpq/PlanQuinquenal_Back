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
    public interface IImpedimentoRepository
    {
        Task<ResponseDTO> Add(ImpedimentoRequestDTO p,DatosUsuario usuario);
        Task<ResponseDTO> Update(ImpedimentoUpdateDto p, DatosUsuario usuario,int id);
        Task<ResponseEntidadDto<ImpedimentoDetalleById>> GetById(int Id);
        Task<DocumentoResponseDto> Download(int id);
        Task<PaginacionResponseDtoException<ImpedimentoDetalle>> Listar(ImpedimentoListDto p);

        Task<ResponseDTO> Documentos(ImpedimentoDocumentoDto p,DatosUsuario usuario);

        Task<PaginacionResponseDtoException<Object>> ListarDocumentos(ListaDocImpedimentosDTO p);
        Task<ImportResponseDto<Impedimento>> ProyectoImport(RequestMasivo data, DatosUsuario usuario);
        Task<ResponseDTO> Delete(int id, DatosUsuario usuario);
        Task<ResponseDTO> DeleteDocumentos(int id, DatosUsuario usuario);
    }
}
