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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class DocumentosRepository: IDocumentosRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public DocumentosRepository(PlanQuinquenalContext context, IConfiguration configuration,IMapper mapper) 
        {
            this._context = context;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<ResponseDTO> Add(DocumentoRequestDto documentoRequestDto, int idUser)
        {
            ResponseDTO obj =new ResponseDTO();
            if (documentoRequestDto.Modulo.Equals("PQ"))
            {
                var data  =await QuinquenalAdd(documentoRequestDto);
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
                var data = await ProyectosAdd(documentoRequestDto);
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
                var data = await PlanAnualAdd(documentoRequestDto);
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
        public async Task<bool> PlanAnualAdd(DocumentoRequestDto documentoRequestDto)
        {
            try
            {
                var resultado = await _context.PlanAnual.Where(x => x.AnioPlan.Equals(documentoRequestDto.CodigoProyecto)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPA>(documentoRequestDto);
                    map.PlanAnualId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] +"\\"+"PlanAnual\\"+ documentoRequestDto.CodigoProyecto+"\\"+guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"]+ "PlanAnual"+ "/" + documentoRequestDto.CodigoProyecto+ "/" + guidId+ Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> QuinquenalAdd(DocumentoRequestDto documentoRequestDto)
        {
            try
            {
                
                var resultado = await _context.PQuinquenal.Where(x => x.AnioPlan.Equals(documentoRequestDto.CodigoProyecto)).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPQ>(documentoRequestDto);
                    map.PQuinquenalId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Quinquenal\\" + documentoRequestDto.CodigoProyecto + "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Quinquenal"+"/" + documentoRequestDto.CodigoProyecto + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();

                    saveDocument(documentoRequestDto, guidId);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }



        }
        public async Task<bool> ProyectosAdd(DocumentoRequestDto documentoRequestDto)
        {
            try
            {
                var resultado = await _context.Proyecto.Where(x => x.CodigoProyecto.Equals(documentoRequestDto.CodigoProyecto) && x.Etapa == documentoRequestDto.Etapa).FirstOrDefaultAsync();
                if (resultado != null)
                {
                    var guidId = Guid.NewGuid();
                    var fecha = DateTime.Now.ToString("ddMMyyy_hhMMss");
                    var map = mapper.Map<DocumentosPy>(documentoRequestDto);
                    map.ProyectoId = resultado.Id;
                    map.CodigoDocumento = documentoRequestDto.NombreDocumento;
                    map.FechaEmision = DateTime.Now;
                    map.Aprobaciones = Convert.ToDateTime(documentoRequestDto.Aprobaciones);
                    map.rutafisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Proyectos\\"+ documentoRequestDto.CodigoProyecto + "_" + documentoRequestDto.Etapa +  "\\" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.NombreDocumento = guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.TipoDocumento = Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Ruta = configuration["DNS"] + "Proyectos" + "/" + documentoRequestDto.CodigoProyecto+"_"+ documentoRequestDto.Etapa + "/" + guidId + Path.GetExtension(documentoRequestDto.NombreDocumento);
                    map.Estado = true;
                    _context.Add(map);
                    await _context.SaveChangesAsync();
                   
                    saveDocument(documentoRequestDto, guidId);
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
        public bool saveDocument(DocumentoRequestDto documentoRequestDto, Guid guidId)
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
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";
               
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                if (documentoRequestDto.Modulo.Equals("PY"))
                {
                    rutaCompleta = ruta + documentoRequestDto.CodigoProyecto+"_"+ documentoRequestDto.Etapa;
                }
                else
                {
                    rutaCompleta = ruta + documentoRequestDto.CodigoProyecto;
                }
                    
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
            return obj;
            
        }

        public async Task<ResponseDTO> Delete(int id, string modulo, int idUser)
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

        public async Task<PaginacionResponseDto<DocumentoResponseDto>> Listar(ListDocumentosRequestDto listDocumentosRequestDto)
        {
            if (listDocumentosRequestDto.Modulo == "PY")
            {
                try
                {
                    var queryable = _context.DocumentosPy
                                    .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
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
                    var queryable = _context.DocumentosPQ
                            .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true)
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
                    var queryable = _context.DocumentosPA
                        .Where(x => listDocumentosRequestDto.Buscar != "" ? x.CodigoDocumento == listDocumentosRequestDto.Buscar : true).AsQueryable();

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
