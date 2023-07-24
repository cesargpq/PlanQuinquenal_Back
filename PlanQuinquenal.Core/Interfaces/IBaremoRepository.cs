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
    public interface IBaremoRepository
    {
        Task<Baremo> GetById(int id);
        Task<Baremo> GetByCodigo(string codigo);
        Task<ImportResponseDto<Baremo>> BaremoImport(RequestMasivo data, DatosUsuario usuario);
        Task<ResponseDTO> Editarbaremo(BaremoRequestDto data, int id, DatosUsuario usuario);
        Task<ResponseDTO> CrearBaremo(BaremoRequestDto data,DatosUsuario usuario);
        Task<bool> ExisteQuinquenal(int id);
        Task<PaginacionResponseDto<Baremo>> GetAll(BaremoListDto entidad);
        Task<ResponseDTO> EliminarBaremo(int id, DatosUsuario usuario);
    }
}
