using ApiDavis.Core.Utilidades;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PermisosRepository : IRepositoryPermisos
    {
        private readonly PlanQuinquenalContext _context;

        public PermisosRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }
        public async Task<ColumsTablaPerResponse> VisColumnTabla(int idUser, string tabla)
        {
            
            ColumsTablaPerResponse respMod = new ColumsTablaPerResponse();
            List<ColumTablaUsuResponseDto> lstpermisoColumUsu = new List<ColumTablaUsuResponseDto>(); 

            var queryable = await _context.ColumTablaUsu.Include(x => x.columTabla)
                                     .Where(x => x.iduser == idUser)
                                     .Where(x => x.columTabla.tabla == tabla)
                                     .ToListAsync();

            foreach(var colums in queryable)
            {
                var permisoColumTabla = new ColumTablaUsuResponseDto
                {
                    id = colums.id,
                    title = colums.columTabla.title,
                    campo = colums.columTabla.campo,
                    tipo = colums.columTabla.tipo,
                    seleccion = colums.seleccion
                };

                lstpermisoColumUsu.Add(permisoColumTabla);
            }

            if (queryable.Count() > 0)
            {

                respMod.idMensaje = "1";
                respMod.mensaje = "Permisos obtenidos correctamente";
                respMod.perm_ColumTabla = lstpermisoColumUsu;
            }
            else
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "Su usuario no cuenta con Permisos de visualizacion de esta tabla";
            }


            return respMod;
        }

        public async Task<Object> ModPermisoColumnTabla(ColumTablaUsu columna, int idUser)
        {
            var modperTabla = _context.ColumTablaUsu.FirstOrDefault(p => p.id == columna.id);
            modperTabla.seleccion = columna.seleccion;
            _context.SaveChanges();
            var resp = new
            {
                idMensaje = "1",
                mensaje = "Se modifico el permiso de la columna correctamente"
            };

            var json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public  async Task<ModulosResponse> ActualizarPermisosMod(TablaPerm_viz_modulo reqModulos)
        {
            ModulosResponse respMod = new ModulosResponse();

            var modperfil =  _context.Perm_viz_modulo.FirstOrDefault(p => p.codMod_permiso == reqModulos.codMod_permiso);
            // Verificar si se encontró la persona
            if (modperfil == null)
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "Su usuario no cuenta con permisos de visualizacion de secciones";
                return respMod;
            }
            else
            {
                // Actualizar los datos de la persona
                modperfil.perm_dashb = reqModulos.perm_dashb;
                modperfil.perm_planQui = reqModulos.perm_planQui;
                modperfil.perm_proyectos = reqModulos.perm_proyectos;
                modperfil.perm_gestRem = reqModulos.perm_gestRem;
                modperfil.perm_infActas = reqModulos.perm_infActas;
                modperfil.perm_reportes = reqModulos.perm_reportes;
                modperfil.perm_admin = reqModulos.perm_admin;

                // Guardar los cambios en la base de datos
                _context.SaveChanges();
                respMod.idMensaje = "1";
                respMod.mensaje = "Se actualizaron sus permisos correctamente";
                return respMod;
            }

            
        }

        public async Task<Object> ActAccionesPerfil(List<Acciones_Rol> reqAccRol)
        {
            try
            {
                foreach (var accRol in reqAccRol)
                {
                    var modAccion_Rol = _context.Acciones_Rol.FirstOrDefault(p => p.id == accRol.id);
                    modAccion_Rol.acc_crear = accRol.acc_crear;
                    modAccion_Rol.acc_ver = accRol.acc_ver;
                    modAccion_Rol.acc_eliminar= accRol.acc_eliminar;
                    _context.SaveChanges();
                }

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico las acciones por rol correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch(Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar los permisos"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<List<Acciones_Rol>> ConsAccionesPerfil()
        {
            List<Acciones_Rol> lstresp =  new List<Acciones_Rol>();
            var queryable = await _context.Acciones_Rol
                                     .ToListAsync();

            lstresp = queryable;
            return lstresp;

        }

        public async Task<Object> ActConfRolesPerm(List<ConfRolesPerm> conRolesPerm)
        {
            try
            {
                foreach (var accRol in conRolesPerm)
                {
                    var modAccion_Rol = _context.Permisos_viz_seccion.FirstOrDefault(p => p.cod_perm_campo == accRol.cod_perm_campo);
                    modAccion_Rol.visib_campo = accRol.visib_campo;
                    modAccion_Rol.edit_campo = accRol.edit_campo;
                    _context.SaveChanges();
                }

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico los permisos correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar los permisos"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<PaginacionResponseDto<dynamic>> ObtenerConfRolesPerm(string modulo, ConfRolesPerm paginacion)
        {
            List<dynamic> listaPermisos = new List<dynamic>();
            var QuerylistaCampos = _context.CamposModulo_Permisos.Where(x => x.modulo == modulo).AsQueryable();
            var listaPerfiles = await _context.Perfil.OrderBy(e => e.cod_perfil).ToListAsync();
            int cantRegist = QuerylistaCampos.Count();
            var listaCampos = await QuerylistaCampos.Paginar(paginacion).ToListAsync();

            foreach (var campo in listaCampos)
            {
                dynamic obj = new ExpandoObject();
                obj.nom_seccion = campo.descripcion;
                foreach (var perfilItem in listaPerfiles)
                {
                    int perfil_sec = perfilItem.Permisos_viz_seccioncodSec_permViz;
                    var listapermisos = await _context.Permisos_viz_seccion.Where(x => x.codSec_permViz == perfil_sec)
                        .Where(x => x.cod_campo == campo.id).ToListAsync();
                    dynamic perfil = new ExpandoObject();
                    perfil.id = listapermisos[0].cod_perm_campo;
                    perfil.edit_campo = listapermisos[0].edit_campo;
                    perfil.nombre = perfilItem.nombre_perfil;
                    string perfilNombre = "Perfil" + perfilItem.cod_perfil;
                    ((IDictionary<string, object>)obj)[perfilNombre] = perfil;
                }

                listaPermisos.Add(obj);
            }

            var objetos = new PaginacionResponseDto<dynamic>
            {
                Cantidad = cantRegist,
                Model = listaPermisos

            };
            return objetos;


        }

        public async Task<PaginacionResponseDto<dynamic>> obtenerPermisosPagina(string nombrePagina, int idUser)
        {
            List<dynamic> listaPermisos = new List<dynamic>();
            var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == idUser).ToListAsync();
            int codigoPerm = Usuario[0].Perfil.Permisos_viz_seccioncodSec_permViz;

            var lstCamposPermiso = await _context.Permisos_viz_seccion.Include(y => y.campo).Where(y => y.codSec_permViz == codigoPerm)
                .Where(y => y.campo.pagina == nombrePagina).ToListAsync();
            foreach (var campo in lstCamposPermiso)
            {
                dynamic obj = new ExpandoObject();
                obj.nombreCampo = campo.campo.nombre_campo;
                obj.flagEdicion = campo.edit_campo;
                listaPermisos.Add(obj);
            }

            var objetos = new PaginacionResponseDto<dynamic>
            {
                Cantidad = 0,
                Model = listaPermisos

            };
            return objetos;
        }

        public async Task<ResponseDTO> actualizarPermisoPerfil(ActualizarPermisoRequestDto permiso)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var modPermiso = _context.Permisos_viz_seccion.FirstOrDefault(p => p.cod_perm_campo == permiso.codigo);
                modPermiso.edit_campo = permiso.flag_edit;
                _context.SaveChanges();

                dto.Message = Constantes.CreacionExistosa;
                dto.Valid = true;
            }
            catch
            {
                dto.Message = Constantes.ErrorSistema;
                dto.Valid = false;
            }
            
            return dto;
        }

    }
}
