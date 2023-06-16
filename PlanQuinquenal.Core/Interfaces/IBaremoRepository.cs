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
    public interface IBaremoRepository
    {
        Task<Baremo> GetById(int id);  
        Task<ImportResponseDto<Baremo>> BaremoImport(RequestMasivo data);
        Task<ResponseDTO> Editarbaremo(BaremoRequestDto data, int id);
        Task<ResponseDTO> CrearBaremo(BaremoRequestDto data);
        Task<bool> ExisteQuinquenal(int id);
        Task<PaginacionResponseDto<Baremo>> GetAll(BaremoListDto entidad);
        Task<ResponseDTO> EliminarBaremo(int id);
    }
}
