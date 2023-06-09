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
        Task<Object> CrearComentario(Comentarios_proyec comentario, int idUser);
        Task<Object> EliminarComentario(int codigo);
        Task<Object> CrearDocumento(Docum_proyecto requestDoc, int idUser);
        Task<Object> EliminarDocumento(int codigo);
        Task<Object> CrearPermiso(Permisos_proyec requestDoc, int idUser);
        Task<Object> EliminarPermiso(int codigo);
        Task<Object> CrearInforme(InformeRequestDTO requestDoc, int idUser);
        Task<Object> ModificarInforme(InformeRequestDTO requestDoc, int idUser);
        Task<Object> EliminarInforme(int codigo);
        Task<Object> CrearActa(ActaRequestDTO requestDoc, int idUser);
        Task<Object> ModificarActa(ActaRequestDTO requestDoc, int idUser);
        Task<Object> EliminarActa(int codigo);
    }
}
