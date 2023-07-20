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
    public interface IProyectoRepository
    {
        //Task<ProyectoResponseDto> GetById(int id);
        Task<PaginacionResponseDtoException<ProyectoDetalle>> GetSeleccionados(PlanQuinquenalSelectedId p);
        Task<ResponseEntidadDto<ProyectoResponseIdDTO>> GetById(int Id);
        Task<PaginacionResponseDtoException<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos);
        Task<PaginacionResponseDtoException<ProyectoDetalle>> GetAll2(FiltersProyectos filterProyectos);
        Task<ResponseDTO> Update(ProyectoRequestUpdateDto p,int id, int idUser);
        Task<ResponseDTO> Add(ProyectoRequestDto proyectoRequestDto,int idUser);
        Task<ImportResponseDto<Proyecto>> ProyectoImport(RequestMasivo data);
    }
}
