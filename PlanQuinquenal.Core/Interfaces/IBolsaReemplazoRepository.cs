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
    public interface IBolsaReemplazoRepository
    {
        Task<ResponseDTO> Add(RequestBolsaDto p, DatosUsuario usuario);
        Task<ResponseDTO> GestionReemplazo(GestionReemplazoDto p, DatosUsuario usuario);
        Task<ResponseDTO> Update(RequestUpdateBolsaDTO p, int id, DatosUsuario usuario);
        Task<PaginacionResponseDtoException<BolsaDetalle>> Listar(BolsaRequestList p);
        Task<ResponseEntidadDto<BolsaDetalleById>> GetById(int id);
        Task<ImportResponseDto<BolsaReemplazo>> ImportarMasivo(RequestMasivo data, DatosUsuario usuario);
    }
}
