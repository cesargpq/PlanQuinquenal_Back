using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryLogin
    {
        Task<LoginResponseDTO> Post(LoginRequestDTO loginRequestDTO);
        Task<ModulosResponse> ObtenerModulos(string correo);
        Task<ModulosResponse> ObtenerSecciones(string modulo, string seccion);
    }
}
