using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
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
        public async Task<ColumsTablaPerResponse> VisColumnTabla(string correo, string tabla)
        {
            ColumsTablaPerResponse respMod = new ColumsTablaPerResponse();

            var queryable = await _context.ColumTablaUsu
                                     .Where(x => x.correo == correo)
                                     .Where(x => x.tabla == tabla)
                                     .ToListAsync();

            if (queryable.Count() > 0)
            {

                respMod.idMensaje = "1";
                respMod.mensaje = "Permisos obtenidos correctamente";
                respMod.perm_ColumTabla = queryable;
            }
            else
            {
                respMod.idMensaje = "0";
                respMod.mensaje = "Su usuario no cuenta con Permisos de visualizacion de esta tabla";
            }


            return respMod;
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

        public async Task<List<ConfRolesPerm>> ObtenerConfRolesPerm()
        {
            List<ConfRolesPerm> lstresp = new List<ConfRolesPerm>();
            var queryable = await _context.Permisos_viz_seccion.Include(p => p.seccionModulo)
                                     .ToListAsync();

            foreach (var permRol in queryable)
            {
                ConfRolesPerm objPerRol = new ConfRolesPerm();
                objPerRol.cod_perm_campo = permRol.cod_perm_campo;
                objPerRol.codSec_permViz = permRol.codSec_permViz;
                objPerRol.cod_seccion = permRol.cod_seccion;
                objPerRol.nom_seccion = permRol.seccionModulo.seccion;
                objPerRol.modulo = permRol.seccionModulo.modulo;
                objPerRol.nom_campo = permRol.nom_campo;
                objPerRol.visib_campo = permRol.visib_campo;
                objPerRol.edit_campo = permRol.edit_campo;

                lstresp.Add(objPerRol);

            }

            return lstresp;

        }

    }
}
