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
    public interface IUsuarioRepository
    {
        Task<PaginacionResponseDto<Usuario>> GetAll(UsuarioListDTO entidad);
        Task<Usuario> GetById(int id);

        Task<ResponseDTO> Update(UsuarioRequestDto usuario,int id);
        Task<ResponseDTO> UpdateState(int id);
        Task<ResponseDTO> DesbloquearUsuario(string correo);
        
        Task<ResponseDTO> CreateUser(UsuarioRequestDto usuario);

    }
}
