using ApiDavis.Core.Utilidades;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly HashService hashService;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public UsuarioRepository(PlanQuinquenalContext context, IMapper mapper, HashService hashService, IRepositoryNotificaciones repositoryNotificaciones) 
        {
            this._context = context;
            this.mapper = mapper;
            this.hashService = hashService;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }

        public async Task<PaginacionResponseDto<Usuario>> GetAll(UsuarioListDTO entidad)
        {
            var usuarios = await _context.Usuario.ToListAsync();

            
            if(entidad.buscar != "")
            {
                 var queryable = _context.Usuario
                                    .Include(x => x.Perfil)
                                     .Include(x => x.Unidad_negocio)
                                     .Where
                                     (x =>
                                     x.nombre_usu.Contains(entidad.buscar )||
                                     x.apellido_usu.Contains(entidad.buscar) ||
                                     x.correo_usu.Contains(entidad.buscar) ||
                                     x.estado_user.Contains(entidad.buscar) ||
                                     x.Perfil.nombre_perfil.Contains(entidad.buscar ))
                                     .AsQueryable();
                var entidades = await queryable.OrderBy(e => e.nombre_usu).Paginar(entidad)
                                   .ToListAsync();
                int cantidad = queryable.Count();
                var objeto = new PaginacionResponseDto<Usuario>
                {
                    Cantidad = cantidad,
                    Model = entidades
                };
                return objeto;
            }
            else
            {
                 var queryable = _context.Usuario
                                     .Include(x => x.Perfil)
                                     .Include(x => x.Unidad_negocio)
                                     .Where(x => entidad.cod_usuario != 0 ? x.cod_usu == entidad.cod_usuario : true)
                                     .Where(x => entidad.nombre_usu != "" ? x.nombre_usu == entidad.nombre_usu : true)
                                     .Where(x => entidad.apellido_usu != "" ? x.apellido_usu == entidad.apellido_usu : true)
                                     .Where(x => entidad.correo_usu != "" ? x.correo_usu == entidad.correo_usu : true)
                                     .Where(x => entidad.estado != "" ? x.estado_user == entidad.estado : true)
                                     .Where(x => entidad.cod_rol != 0 ? x.cod_rol == entidad.cod_rol : true)
                                     .Where(x => entidad.cod_perfil != 0 ? x.Perfilcod_perfil == entidad.cod_perfil : true)
                                     .AsQueryable();

                var entidades = await queryable.OrderBy(e => e.cod_usu).Paginar(entidad)
                                   .ToListAsync();
                int cantidad = queryable.Count();
                var objeto = new PaginacionResponseDto<Usuario>
                {
                    Cantidad = cantidad,
                    Model = entidades

                };
                return objeto;
            }            
        }

        public async Task<Usuario> GetById(int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync( x => x.cod_usu == id);
            resultado.passw_user = hashService.Desencriptar(resultado.passw_user);
            return resultado;
        }

        public async Task<ResponseDTO> Update(UsuarioRequestDto usuario, int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync(x => x.cod_usu == id);
            ResponseDTO resp = new ResponseDTO();
            if (resultado != null)
            {
                resultado.nombre_usu = usuario.nombre_usu;
                resultado.apellido_usu = usuario.apellido_usu;
                resultado.passw_user = hashService.Encriptar(usuario.passw_user);
                resultado.correo_usu = usuario.correo_usu;
                resultado.cod_rol = usuario.cod_rol;
                resultado.Perfilcod_perfil = usuario.Perfilcod_perfil;
                resultado.Interno = usuario.Interno;
                var perfil = await _context.Perfil.Where(x=>x.cod_perfil== usuario.Perfilcod_perfil).FirstOrDefaultAsync();
                resultado.DobleFactor = usuario.DobleFactor;
                resultado.Unidad_negociocod_und = perfil.cod_unidadNeg;
                _context.Update(resultado);
                await _context.SaveChangesAsync();
                resp.Message = Constantes.ActualizacionSatisfactoria;
                resp.Valid = true;
                return resp;
            }
            else
            {
                resp.Message = Constantes.ActualizacionError;
                resp.Valid = false;
                return resp;
            }
        }
        public async Task<ResponseDTO> DesbloquearUsuario(string correo)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync(x => x.correo_usu == correo);
            ResponseDTO resp = new ResponseDTO();
            if (resultado != null)
            {
                resultado.Intentos = 0;
                resultado.Estado = true;
                _context.Update(resultado);
                await _context.SaveChangesAsync();
                resp.Message = Constantes.ActualizacionSatisfactoria;
                resp.Valid = true;
                return resp;
            }
            else
            {
                resp.Message = Constantes.ActualizacionError;
                resp.Valid = false;
                return resp;
            }
        }
        public async Task<ResponseDTO> UpdateState(int id)
        {
            var resultado = await _context.Usuario.FirstOrDefaultAsync(x => x.cod_usu == id);
            ResponseDTO resp = new ResponseDTO();
            if (resultado != null)
            {
                resultado.estado_user = resultado.estado_user == "A" ? "D" : "A";
                resultado.passw_user = hashService.Desencriptar(resultado.passw_user);
                _context.Update(resultado);
                await _context.SaveChangesAsync();
                resp.Message = Constantes.EliminacionSatisfactoria;
                resp.Valid = true;
                return resp;
            }
            else
            {
                resp.Message = Constantes.ActualizacionError;
                resp.Valid = false;
                return resp;
            }
        }
        public async Task<ResponseDTO> CreateUser(UsuarioRequestDto usuario)
        {
            var existe = await _context.Usuario.Where(x => x.correo_usu.Equals(usuario.correo_usu)).FirstOrDefaultAsync();
            ResponseDTO resp = new ResponseDTO();

            if (existe!= null)
            {
           
                resp.Valid = false;
                resp.Message = Constantes.ExisteRegistro;
                return resp;
            }

            resp.Valid = true;
            resp.Message = Constantes.CreacionExistosa;
            var perfil = await _context.Perfil.Where(x => x.cod_perfil == usuario.Perfilcod_perfil).FirstOrDefaultAsync();
            var usuarioDto = mapper.Map<Usuario>(usuario);
            usuarioDto.passw_user = hashService.Encriptar(usuarioDto.passw_user);
            usuarioDto.estado_user = "A";
            usuarioDto.Estado = true;
            usuarioDto.Intentos = 0;
            usuarioDto.LastSesion = null;
            usuarioDto.Interno = usuario.Interno;
            usuarioDto.Conectado = false;
            usuarioDto.Unidad_negociocod_und = perfil.cod_unidadNeg;
            usuarioDto.FechaCreacion = DateTime.Now;
            usuarioDto.FechaModifica = null;
            _context.Add(usuarioDto);

            await _context.SaveChangesAsync();

            #region creacion de permisos de visualizacion de columnas de todas las tablas

            var QuerylistaColums = _context.ColumnasTablas.AsQueryable();
            var listaColums = await QuerylistaColums.ToListAsync();
            List<ColumTablaUsu> listaInsert = new List<ColumTablaUsu>();
            foreach (var campo in listaColums)
            {
                var nuevoPermiso = new ColumTablaUsu
                {
                    seleccion = true,
                    iduser = usuarioDto.cod_usu,
                    idColum = campo.id
                };

                listaInsert.Add(nuevoPermiso);
                //_context.ColumTablaUsu.Add(nuevoPermiso);
                //_context.SaveChanges();
            }
            await _context.BulkInsertAsync(listaInsert);
            await _context.SaveChangesAsync();

            #endregion

            #region creacion de permisos de notificaciones 
            var nuevaConfNotificacion = new Config_notificaciones
            {
                cod_usu = usuarioDto.cod_usu,
                regPQ = true,
                modPQ = true,
                regPry = true,
                modPry = true,
                regImp = true,
                modImp = true,
                regPer = true,
                modPer = true,
                regEviReemp = true,
                modEviReemp = true,
                regCom = true,
                modCom = true,
                regInfActas = true,
                modInfActas = true
            };
            await _repositoryNotificaciones.CrearConfigNotif(nuevaConfNotificacion);
            #endregion
            return resp;
        }
    }
}
