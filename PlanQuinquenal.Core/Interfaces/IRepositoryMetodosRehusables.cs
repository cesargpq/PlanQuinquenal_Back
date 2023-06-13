using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Core.Interfaces
{
    public interface IRepositoryMetodosRehusables
    {
        Task<Object> CrearComentario(Comentarios_proyecDTO comentario, int idUser, string modulo);
        Task<Object> EliminarComentario(int codigo, string modulo);
        Task<Object> CrearDocumento(Docum_proyectoDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarDocumento(int codigo, string modulo);
        Task<Object> CrearPermiso(Permisos_proyecDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarPermiso(int codigo, string modulo);
        Task<Object> CrearInforme(InformeRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> ModificarInforme(InformeRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarInforme(int codigo, string modulo);
        Task<Object> CrearActa(ActaRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> ModificarActa(ActaRequestDTO requestDoc, int idUser, string modulo);
        Task<Object> EliminarActa(int codigo, string modulo);
    }
}
