using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryPermisos
    {
        Task<ColumsTablaPerResponse> VisColumnTabla(int idUser, string tabla);
        Task<ModulosResponse> ActualizarPermisosMod(TablaPerm_viz_modulo reqModulos);
        Task<Object> ActAccionesPerfil(List<Acciones_Rol> reqAccRol);
        Task<List<Acciones_Rol>> ConsAccionesPerfil();
        Task<Object> ObtenerConfRolesPerm(int modulo);
        Task<Object> ActConfRolesPerm(List<ConfRolesPerm> conRolesPerm);
        Task<Object> ModPermisoColumnTabla(ColumTablaUsu columna, int idUser);
    }
}
