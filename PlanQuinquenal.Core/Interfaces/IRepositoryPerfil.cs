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
    public interface IRepositoryPerfil
    {
        Task<Object> NuevoPerfil(PerfilResponse nvoPerfil);
        Task<PaginacionResponseDto<PerfilResponse>> ObtenerPerfiles(PerfilListDto buscador);
        Task<Object> EliminarPerfil(int cod_perfil);
        Task<Object> ActualizarPerfil(PerfilResponse nvoPerfil);
    }
}
