using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using ApiDavis.Core.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanQuinquenal.Core.DTOs;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class DocumentosRepository: IDocumentosRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ITrazabilidadRepository _trazabilidadRepository;

        public DocumentosRepository(PlanQuinquenalContext context, IConfiguration configuration,IMapper mapper, ITrazabilidadRepository trazabilidadRepository) 
        {
            this._context = context;
            this.configuration = configuration;
            this.mapper = mapper;
            this._trazabilidadRepository = trazabilidadRepository;
        }

        public async Task<ResponseDTO> Add(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            ResponseDTO obj =new ResponseDTO();
            if (documentoRequestDto.Modulo.Equals("PQ"))
            {
                var data  =await QuinquenalAdd(documentoRequestDto,usuario);

                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }
            else if (documentoRequestDto.Modulo.Equals("PY"))
            {
                var data = await ProyectosAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    
                }
                
            }
            else if (documentoRequestDto.Modulo.Equals("PA"))
            {
                var data = await PlanAnualAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }
            else if (documentoRequestDto.Modulo.Equals("BR"))
            {
                var data = await BolsaReemplazoAdd(documentoRequestDto, usuario);
                if (data)
                {
                    obj.Valid = true;
                    obj.Message = Constantes.CreacionExistosa;
                }
                else
                {
                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;

                }
            }

            
            return obj;
        }

        public async Task<bool> BolsaReemplazoAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.BolsaReemplazo.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosBR>(documentoRequestDto);
                    map.BolsaReemplazoId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Reemplazo\\" + resultado.CodigoProyecto + "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.ruta = configuration["DNS"] + "Reemplazo" + "/" + resultado.CodigoProyecto + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId,resultado.CodigoProyecto);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PlanAnualAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.PlanAnual.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPA>(documentoRequestDto);
                    map.PlanAnualId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] +"\\"+"PlanAnual\\"+ resultado.AnioPlan+"\\"+guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"]+ "PlanAnual"+ "/" + resultado.AnioPlan + "/" + guidId+ Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId,resultado.AnioPlan);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> QuinquenalAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                
                var resultado = await _context.PQuinquenal.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPQ>(documentoRequestDto);
                    map.PQuinquenalId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + resultado.AnioPlan + "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Quinquenal"+"/" + resultado.AnioPlan + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();


                    saveDocument(documentoRequestDto, guidId, resultado.AnioPlan);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> ProyectosAdd(DocumentoRequestDto documentoRequestDto, DatosUsuario usuario)
        {
            try
            {
                var resultado = await _context.Proyecto.Where(x => x.Id.Equals(documentoRequestDto.ProyectoId)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPy>(documentoRequestDto);
                    map.ProyectoId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\"+ resultado.CodigoProyecto +   "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Proyectos" + "/" + resultado.CodigoProyecto+ "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();
                   
                    saveDocument(documentoRequestDto, guidId,resultado.CodigoProyecto);
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Documento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el documento  {map.CodigoDocumento}";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception e)
            {
                return false;
            }
           
            
           
        }
        public bool saveDocument(DocumentoRequestDto documentoRequestDto, Guid guidId,string CodigoProyecto)
        {
            try
            {
                string rutaCompleta = "";
                string ruta = "";
                string modulo = "";
                if (documentoRequestDto.Modulo.Equals("PY"))
                {
                    modulo = "Proyectos";
                }
                else if (documentoRequestDto.Modulo.Equals("PQ"))
                {
                    modulo = "Quinquenal";
                }
                else if (documentoRequestDto.Modulo.Equals("PA"))
                {
                    modulo = "PlanAnual";
                }
                else if (documentoRequestDto.Modulo.Equals("BR"))
                {
                    modulo = "Reemplazo";
                }
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";
               
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

               
               
                    rutaCompleta = ruta + CodigoProyecto;
                
                    
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

        public async Task<ResponseDTO> GetUrl(int id, string modulo)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {
                
                if (modulo.Equals("PA"))
                {
                    var data = await _context.DocumentosPA.Where(x => x.Id == id && x.Estado==true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                else if (modulo.Equals("PY"))
                {
                    var data = await _context.DocumentosPy.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                    
                }
                else if (modulo.Equals("PQ"))
                {
                    var data = await _context.DocumentosPQ.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.Ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                else if (modulo.Equals("BR"))
                {
                    var data = await _context.DocumentosBR.Where(x => x.Id == id && x.Estado == true).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        obj.Valid = true;
                        obj.Message = data.ruta;
                    }
                    else
                    {
                        obj.Valid = false;
                        obj.Message = Constantes.NOEXISTEDOCUMENTO;
                    }
                }
                return obj;
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
            
           
        }
        public async Task<DocumentoResponseDto> Download(int id, string modulo)
        {
            DocumentoResponseDto obj = new DocumentoResponseDto();
            if (modulo.Equals("PA"))
            {
                var dato = await _context.DocumentosPA.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("PY"))
            {
                var dato = await _context.DocumentosPy.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("PQ"))
            {
                var dato = await _context.DocumentosPQ.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutafisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            else if (modulo.Equals("BR"))
            {
                var dato = await _context.DocumentosBR.Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.tipoDocumento = dato.TipoDocumento;
                obj.ruta = dato.rutaFisica;
                obj.nombreArchivo = dato.NombreDocumento;
            }
            return obj;
            
        }

        public async Task<ResponseDTO> Delete(int id, string modulo,DatosUsuario usuario)
        {
            ResponseDTO obj = new ResponseDTO();
            try
            {
                try
                {
                    if (modulo.Equals("PA"))
                    {
                        var dato = await _context.DocumentosPA.Where(x => x.Id == id).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;

                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }
                        return obj;

                    }
                    else if (modulo.Equals("PY"))
                    {
                        var dato = await _context.DocumentosPy.Where(x => x.Id == id).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }
                        return obj;
                    }
                    else if (modulo.Equals("PQ"))
                    {
                        var dato = await _context.DocumentosPQ.Where(x => x.Id == id).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }
                        return obj;
                    }
                    else if (modulo.Equals("BR"))
                    {
                        var dato = await _context.DocumentosBR.Where(x => x.Id == id).FirstOrDefaultAsync();
                        dato.Estado = false;
                        _context.Update(dato);
                        await _context.SaveChangesAsync();
                        obj.Valid = true;
                        obj.Message = Constantes.EliminacionSatisfactoria;
                        var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Documento , Eliminar").ToListAsync();
                        if (resultad.Count > 0)
                        {
                            Trazabilidad trazabilidad = new Trazabilidad();
                            List<Trazabilidad> listaT = new List<Trazabilidad>();
                            trazabilidad.Tabla = "Documento";
                            trazabilidad.Evento = "Eliminar";
                            trazabilidad.DescripcionEvento = $"Se eliminó correctamente el docuento {dato.CodigoDocumento} ";
                            trazabilidad.UsuarioId = usuario.UsuaroId;
                            trazabilidad.DireccionIp = usuario.Ip;
                            trazabilidad.FechaRegistro = DateTime.Now;

                            listaT.Add(trazabilidad);
                            await _trazabilidadRepository.Add(listaT);
                        }
                        return obj;
                    }

                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    return obj;
                }
                catch (Exception e)
                {

                    obj.Valid = true;
                    obj.Message = Constantes.ErrorSistema;
                    return obj;
                }
               
               
            }
            catch (Exception)
            {

                obj.Valid = false;
                obj.Message = Constantes.ErrorSistema;
                return obj;
            }
            
            
        }

        public async Task<PaginacionResponseDto<DocumentoResponseDto>> Listar(ListDocumentosRqDTO listDocumentosRequestDto)
        {
            if (listDocumentosRequestDto.Modulo == "PY")
            {
                try
                {
                    var proyecto = await _context.Proyecto.Where(x => x.Id.Equals(listDocumentosRequestDto.ProyectoId) ).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPy
                                    .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                                    .Where(x=>x.ProyectoId == proyecto.Id)
                                    .Where(x=> x.Estado==true)
                                    .OrderBy(x=>x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto  >>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e )
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }
               
            }
            else if (listDocumentosRequestDto.Modulo == "PQ")
            {
                try
                {
                    var pq = await _context.PQuinquenal.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPQ
                            .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                            .Where(x => x.PQuinquenalId == pq.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();
                       

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }
            else if (listDocumentosRequestDto.Modulo == "PA")
            {
                try
                {
                    var pa = await _context.PlanAnual.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosPA
                        .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                        .Where(x => x.PlanAnualId == pa.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }
            else if (listDocumentosRequestDto.Modulo == "BR")
            {
                try
                {
                    var br = await _context.BolsaReemplazo.Where(x => x.Id == listDocumentosRequestDto.ProyectoId).FirstOrDefaultAsync();
                    var queryable = _context.DocumentosBR
                        .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
                        .Where(x => x.BolsaReemplazoId == br.Id)
                             .Where(x => x.Estado == true)
                                    .OrderBy(x => x.FechaEmision)
                                    .AsQueryable();

                    var entidades = await queryable.Paginar(listDocumentosRequestDto)
                                       .ToListAsync();

                    var map = mapper.Map<List<DocumentoResponseDto>>(entidades);
                    int cantidad = queryable.Count();
                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = cantidad,
                        Model = map
                    };
                    return objeto;
                }
                catch (Exception e)
                {

                    var objeto = new PaginacionResponseDto<DocumentoResponseDto>
                    {
                        Cantidad = 0,
                        Model = null
                    };
                    return objeto;
                }

            }

            throw new NotImplementedException();
        }
    }
}
