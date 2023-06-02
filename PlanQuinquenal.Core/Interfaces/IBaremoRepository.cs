using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IBaremoRepository
    {
        Task<ImportResponseDto> BaremoImport(ProyectoRequest data);
        Task<ResponseDTO> Editarbaremo(BaremoRequestDto data, int id);
        Task<ResponseDTO> CrearBaremo(BaremoRequestDto data);
        Task<ResponseDTO> EliminarBaremo(int id);
    }
}
