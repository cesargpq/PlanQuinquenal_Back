using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using ApiDavis.Core.Utilidades;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PermisosProyectoRepository : IPermisosProyectoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public PermisosProyectoRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public async Task<ResponseDTO> Add(PermisoRequestDTO permisoRequestDTO, int idUser)
        {
            try
            {
                var existeProyecto = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(permisoRequestDTO.CodigoProyecto)).FirstOrDefaultAsync();


                if (existeProyecto != null)
                {
                    var obtenerPermiso = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(permisoRequestDTO.TipoPermisosProyecto.ToUpper())).FirstOrDefaultAsync();
                    if (obtenerPermiso == null)
                    {
                        return new ResponseDTO
                        {
                            Valid = false,
                            Message = "El permiso envíado no existe"
                        };
                    }
                    var permisoExiste = await _context.PermisosProyecto.Where(x => x.ProyectoId == existeProyecto.Id && x.TipoPermisosProyectoId == obtenerPermiso.Id).FirstOrDefaultAsync();
                    if (permisoExiste!=null)
                    {
                        permisoExiste.Longitud = permisoRequestDTO.Longitud;
                        permisoExiste.EstadoPermisosId = permisoRequestDTO.EstadoPermisosId;
                        _context.Update(permisoExiste);
                        await _context.SaveChangesAsync();
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = Constantes.ActualizacionSatisfactoria
                        };
                    }
                    else
                    {
                        PermisosProyecto obj = new PermisosProyecto();

                        obj.ProyectoId = existeProyecto.Id;
                        obj.TipoPermisosProyectoId = obtenerPermiso.Id;
                        obj.Longitud = permisoRequestDTO.Longitud;
                        obj.EstadoPermisosId = permisoRequestDTO.EstadoPermisosId;
                        obj.Estado = true;
                        obj.FechaCreacion = DateTime.Now;
                        obj.FechaModificacion = DateTime.Now;
                        obj.UsuarioCreacion = idUser;
                        obj.UsuarioModifca = idUser;
                        _context.Add(obj);
                        await _context.SaveChangesAsync();
                        return new ResponseDTO
                        {
                            Valid = true,
                            Message = Constantes.CreacionExistosa
                        };
                    }
                    
                }
                else
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = "El proyecto y la etapa ingresado no existe"
                    };
                };
            }
            catch (Exception e )
            {

                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
            }
        }
        public async Task<ResponseEntidadDto<PermisoByIdResponseDto>> GetPermiso(int idProyecto, string TipoPermiso)
        {
            var tipoPerm = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(TipoPermiso.ToUpper())).FirstOrDefaultAsync();
            if (tipoPerm == null)
            {
                return new ResponseEntidadDto<PermisoByIdResponseDto>
                {
                    Model = null,
                    Valid = true,
                    Message = "No existe al tipo de permiso envíado"
                };
            }
            else
            {
                var resultado = await _context.PermisosProyecto.Where(x => x.ProyectoId == idProyecto && x.TipoPermisosProyectoId==tipoPerm.Id).FirstOrDefaultAsync();

                if(resultado == null)
                {
                    return new ResponseEntidadDto<PermisoByIdResponseDto>
                    {
                        Model = null,
                        Valid = true,
                        Message = "No existe el permiso solicitado"
                    };
                }
                else
                {
                    var map = mapper.Map<PermisoByIdResponseDto>(resultado);
                    return new ResponseEntidadDto<PermisoByIdResponseDto>
                    {
                        Model = map,
                        Valid = true,
                        Message = Constantes.BusquedaExitosa
                    };
                }
            }
            
        }

        public async Task<ResponseDTO> CargarExpediente(DocumentosPermisosRequestDTO documentosPermisosRequestDTO, int idUser)
        {
            var existeProyecto = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(documentosPermisosRequestDTO.CodigoProyecto)).FirstOrDefaultAsync();

            if(existeProyecto != null)
            {
                var tipoPerm = await _context.TipoPermisosProyecto.Where(x => x.Descripcion.ToUpper().Equals(documentosPermisosRequestDTO.TipoPermisosProyecto.ToUpper())).FirstOrDefaultAsync();
                
                DocumentosPermisos documentos = new DocumentosPermisos();

                var guidId = Guid.NewGuid();
                var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                documentos.ProyectoId = existeProyecto.Id;
                documentos.TipoPermisosProyectoId = tipoPerm.Id;
                documentos.NombreDocumento = documentosPermisosRequestDTO.NombreDocumento;
                documentos.CodigoDocumento = guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.Fecha = Convert.ToDateTime(documentosPermisosRequestDTO.Fecha);
                documentos.Expediente = documentosPermisosRequestDTO.Expediente;
                documentos.rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" +"Proyectos\\"+  documentosPermisosRequestDTO.CodigoProyecto+"_"+documentosPermisosRequestDTO.Etapa + $"\\Permiso\\{tipoPerm.Descripcion}\\" + guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.ruta = configuration["DNS"] + "Proyectos" + "/" + documentosPermisosRequestDTO.CodigoProyecto + "_" + documentosPermisosRequestDTO.Etapa+ $"/Permiso/{tipoPerm.Descripcion}/" + guidId + Path.GetExtension(documentosPermisosRequestDTO.NombreDocumento);
                documentos.FechaCreacion = DateTime.Now;
                documentos.FechaModificacion = DateTime.Now;
                documentos.UsuarioCreacion = idUser;
                documentos.UsuarioModifca = idUser;
                documentos.Estado = true;
                documentos.Vencimiento = documentosPermisosRequestDTO.Vencimiento!="" || documentosPermisosRequestDTO.Vencimiento != null ? DateTime.Parse(documentosPermisosRequestDTO.Vencimiento):null;
                _context.Add(documentos);
                await _context.SaveChangesAsync();

                saveDocument(documentosPermisosRequestDTO, guidId, tipoPerm.Descripcion);
                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
            }
            else
            {

                return new ResponseDTO
                {
                    Valid = false,
                    Message = "No existe el proyecto y etapa solicitado"
                };

            }
            
        }
         public bool saveDocument(DocumentosPermisosRequestDTO documentoRequestDto, Guid guidId,string tipopermiso)
        {
            try
            {
                string rutaCompleta = "";
                string ruta = "";
                string modulo = "Proyectos";
               
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";
               
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                
                rutaCompleta = ruta + documentoRequestDto.CodigoProyecto+"_"+ documentoRequestDto.Etapa +$"\\Permiso\\{tipopermiso}\\";
                                    
                if (!Directory.Exists(rutaCompleta))
                {
                    Directory.CreateDirectory(rutaCompleta);
                }
                string rutaSave = Path.Combine(rutaCompleta, guidId+Path.GetExtension(documentoRequestDto.NombreDocumento));

                byte[] decodedBytes = Convert.FromBase64String(documentoRequestDto.base64);
                File.WriteAllBytes(rutaSave, decodedBytes);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public async Task<ResponseDTO> Delete(int id,int idUser)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {

                var dato = await _context.DocumentosPermisos.Where(x => x.Id == id).FirstOrDefaultAsync();
                dato.Estado = false;
                dato.UsuarioModifca = idUser;
                dato.FechaModificacion = DateTime.Now;
                _context.Update(dato);
                await _context.SaveChangesAsync();

                obj.Valid = true;
                obj.Message = Constantes.EliminacionSatisfactoria;
                return obj;
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
        }

        public async Task<PaginacionResponseDto<DocumentoPermisosResponseDTO>> Listar(ListDocumentosRequestDto listDocumentosRequestDto)
        {
            try
            {
                var dato = await _context.TipoPermisosProyecto.Where(x=>x.Descripcion.ToUpper().Equals(listDocumentosRequestDto.Modulo.ToUpper())).FirstOrDefaultAsync();
                if (dato!= null)
                {
                    var queryable = _context.DocumentosPermisos
                                .Where(x => listDocumentosRequestDto.Buscar != "" ? x.Expediente.Contains(listDocumentosRequestDto.Buscar) : true)
                                .Where(x=>x.TipoPermisosProyectoId == dato.Id)
                                .Where(x => x.ProyectoId == listDocumentosRequestDto.ProyectoId)
                                .Where(x=>x.Estado ==true)
                                .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoPermisosResponseDTO>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                else
                {
                    var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                    {
                        Cantidad = 0,
                        Model = null
                    };

                    return objeto;
                }
                
            }
            catch (Exception e)
            {

                var objeto = new PaginacionResponseDto<DocumentoPermisosResponseDTO>
                {
                    Cantidad = 0,
                    Model = null
                };
                return objeto;
            }
        }

        public async Task<ResponseEntidadDto<DocumentoPermisosResponseDTO>> Download(int id)
        {
            try
            {
                DocumentoPermisosResponseDTO obj = new DocumentoPermisosResponseDTO();
                var dato = await _context.DocumentosPermisos.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.ruta = dato.rutaFisica;
                obj.NombreDocumento = dato.NombreDocumento;
                obj.CodigoDocumento = dato.CodigoDocumento;

                return new ResponseEntidadDto<DocumentoPermisosResponseDTO>
                {
                    Model = obj,
                    Message = Constantes.RegistroExiste,
                    Valid = true
                };

            }
            catch (Exception)
            {

                return new ResponseEntidadDto<DocumentoPermisosResponseDTO>
                {
                    Model = null,
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false
                };
            }
           
        }
    }
}
