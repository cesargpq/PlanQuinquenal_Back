using ApiDavis.Core.Utilidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PerfilRepository : IRepositoryPerfil
    {
        private readonly PlanQuinquenalContext _context;

        public PerfilRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }

        public async Task<Object> NuevoPerfil(PerfilResponse nvoPerfil)
        {
            try
            {
                // Crear primero el registro en la tabla de roles
                var nuevoRol = new Roles
                {
                    nom_rol = nvoPerfil.nombre_perfil,
                    estado_rol = "A"
                };

                _context.Roles.Add(nuevoRol);
                _context.SaveChanges();

                // Consulta del Rol creado

                var queryable = _context.Roles
                                     .Where(x => x.nom_rol == nvoPerfil.nombre_perfil)
                                     .AsQueryable();
                double cantidad = await queryable.CountAsync();
                if (cantidad == 1)
                {
                    var queryCons = await _context.Roles
                                     .Where(x => x.nom_rol == nvoPerfil.nombre_perfil)
                                     .ToListAsync();
                    // Crear una nueva instancia de la entidad
                    var nPerfil = new Perfil
                    {
                        cod_rol = queryCons[0].cod_rol,
                        Perm_viz_modulocodMod_permiso = queryCons[0].cod_rol,
                        Permisos_viz_seccioncodSec_permViz = queryCons[0].cod_rol,
                        nombre_perfil = nvoPerfil.nombre_perfil,
                        estado_perfil = nvoPerfil.estado_perfil,
                        cod_unidadNeg = nvoPerfil.cod_unidadNeg
                    };

                    // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                    _context.Perfil.Add(nPerfil);
                    _context.SaveChanges();

                    var resp = new
                    {
                        idMensaje = "1",
                        mensaje = "Se creo el perfil correctamente"
                    };

                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }
                else
                {
                    var resp = new
                    {
                        idMensaje = "0",
                        mensaje = "Hubo un error al crear el perfil"
                    };

                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }

                
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el perfil"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<PaginacionResponseDto<PerfilResponse>> ObtenerPerfiles(PerfilListDto entidad)
        {
            List<PerfilResponse> lstPerfil = new List<PerfilResponse>();
            if (entidad.buscador == "")
            {
                var queryable =  _context.Perfil
                                        .Where(x => entidad.UnidadNegocioId != 0 ? x.cod_unidadNeg == entidad.UnidadNegocioId : true)
                                        .AsQueryable();

                var entidades = await queryable.OrderBy(e => e.nombre_perfil).Paginar(entidad).ToListAsync();

                foreach (var perfil in entidades)
                {
                    PerfilResponse objPerfil = new PerfilResponse();
                    objPerfil.cod_perfil = perfil.cod_perfil;
                    objPerfil.cod_rol = perfil.cod_rol;
                    objPerfil.Perm_viz_modulocodMod_permiso = perfil.Perm_viz_modulocodMod_permiso;
                    objPerfil.Permisos_viz_seccioncodSec_permViz = perfil.Permisos_viz_seccioncodSec_permViz;
                    objPerfil.nombre_perfil = perfil.nombre_perfil;
                    objPerfil.estado_perfil = perfil.estado_perfil;
                    objPerfil.cod_unidadNeg = perfil.cod_unidadNeg;

                    lstPerfil.Add(objPerfil);

                }
                int cantidad = queryable.Count();
                var objetos = new PaginacionResponseDto<PerfilResponse>
                {
                    Cantidad = cantidad,
                    Model = lstPerfil

                };
                return objetos;
            }
            else
            {
                var queryable =  _context.Perfil.Where(x => x.nombre_perfil.Equals(entidad.buscador)).AsQueryable();

                var entidades = await queryable.OrderBy(e => e.nombre_perfil).Paginar(entidad).ToListAsync();

                foreach (var perfil in entidades)
                {
                    PerfilResponse objPerfil = new PerfilResponse();
                    objPerfil.cod_perfil = perfil.cod_perfil;
                    objPerfil.cod_rol = perfil.cod_rol;
                    objPerfil.Perm_viz_modulocodMod_permiso = perfil.Perm_viz_modulocodMod_permiso;
                    objPerfil.Permisos_viz_seccioncodSec_permViz = perfil.Permisos_viz_seccioncodSec_permViz;
                    objPerfil.nombre_perfil = perfil.nombre_perfil;
                    objPerfil.estado_perfil = perfil.estado_perfil;
                    objPerfil.cod_unidadNeg = perfil.cod_unidadNeg;

                    lstPerfil.Add(objPerfil);

                }
                int cantidad = queryable.Count();
                var objetos = new PaginacionResponseDto<PerfilResponse>
                {
                    Cantidad = cantidad,
                    Model = lstPerfil

                };
                return objetos;
            }

           

            var objeto = new PaginacionResponseDto<PerfilResponse>
            {
                Cantidad = 0,
                Model = lstPerfil

            };
            return objeto;

        }

        public async Task<Object> EliminarPerfil(int cod_perfil)
        {
            // Obtener la entidad a eliminar por su clave primaria
            var codPerfilElim = _context.Perfil.Find(cod_perfil);

            // Verificar si se encontró la entidad
            if (codPerfilElim != null)
            {
                // Eliminar la entidad y guardar los cambios en la base de datos
                _context.Perfil.Remove(codPerfilElim);
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se eliminó el perfil correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            else
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo el problema al eliminar el perfil"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<Object> ActualizarPerfil(PerfilResponse nvoPerfil)
        {
            try
            {
                var modPerfil = _context.Perfil.FirstOrDefault(p => p.cod_perfil == nvoPerfil.cod_perfil);
                modPerfil.Perm_viz_modulocodMod_permiso = nvoPerfil.Perm_viz_modulocodMod_permiso;
                modPerfil.Permisos_viz_seccioncodSec_permViz = nvoPerfil.Permisos_viz_seccioncodSec_permViz;
                modPerfil.nombre_perfil = nvoPerfil.nombre_perfil;
                modPerfil.estado_perfil = nvoPerfil.estado_perfil;
                modPerfil.cod_unidadNeg = nvoPerfil.cod_unidadNeg;
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico el perfil correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar el perfil"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }
    }
}
