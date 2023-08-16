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
        Task<LoginResponseDTO> Post(string correo);
        Task<ModulosResponse> ObtenerModulos(string correo);
        Task<ModulosResponse> ObtenerSecciones(string modulo, string seccion);

        Task<bool> VerificaDobleFactor(DFactorDTO dFactorDTO,int idUser);
    }
}
