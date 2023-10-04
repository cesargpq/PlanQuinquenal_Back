using ApiDavis.Core.Utilidades;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using RazorEngine.Templating;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class BaremoRepository : IBaremoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryMantenedores _repositoryMantenedores;
        private readonly ITrazabilidadRepository _trazabilidadRepository;

        public BaremoRepository(PlanQuinquenalContext context, IMapper mapper, IRepositoryMantenedores repositoryMantenedores, ITrazabilidadRepository trazabilidadRepository) 
        {
            this._context = context;
            this.mapper = mapper;
            this._repositoryMantenedores = repositoryMantenedores;
            this._trazabilidadRepository = trazabilidadRepository;
        }

        public async Task<Baremo> GetById(int id)
        {
            var baremo = await _context.Baremo.Where(x=>x.Id == id).FirstOrDefaultAsync();
            return baremo;
        }

        public async Task<Baremo> GetByCodigo(string codigo)
        {
            var baremo = await _context.Baremo.Where(x => x.CodigoBaremo == codigo).FirstOrDefaultAsync();
            return baremo;
        }
        public async Task<ImportResponseDto<Baremo>> BaremoImport(RequestMasivo data, DatosUsuario usuario)
        {
            ImportResponseDto<Baremo> dto = new ImportResponseDto<Baremo>();
            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<Baremo> listaBaremo = new List<Baremo>();
            List<Baremo> listaBaremoError = new List<Baremo>();
            List<Baremo> listaBaremoRepetidos = new List<Baremo>();
            List<Baremo> listaBaremoInsert = new List<Baremo>();
            var PlanQuin = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");
            //var Baremos = await _repositoryMantenedores.GetAllByAttribute("Baremo");
            var Baremos = await _context.Baremo.ToListAsync();
            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Baremo"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var valor1 = worksheet.Cells[row, 1].Value?.ToString();
                            var valor2 = worksheet.Cells[row, 2].Value?.ToString();
                            var valor3 = worksheet.Cells[row, 3].Value?.ToString();
                            var valor4 = worksheet.Cells[row, 4].Value?.ToString();
                            
                            var dato = PlanQuin.Where(x => x.Descripcion == valor4).FirstOrDefault();
                            if(dato == null)
                            {
                                var entidadError = new Baremo
                                {
                                    CodigoBaremo = valor1,
                                    Descripcion = "",
                                    Precio = 0
                                };
                                listaBaremoError.Add(entidadError);
                                break;
                            }
                          
                            try
                            {
                                var entidad = new Baremo
                                {
                                    CodigoBaremo = valor1,
                                    Descripcion = valor2,
                                    Precio = Convert.ToDecimal(valor3),
                                    Estado = true,
                                    PQuinquenalId = dato.Id
                                };
                                listaBaremo.Add(entidad);
                            }
                            catch (Exception e)
                            {
                                var entidadError = new Baremo
                                {
                                    CodigoBaremo = valor1,
                                    Descripcion = "",
                                    Precio = 0
                                };
                                listaBaremoError.Add(entidadError);

                            }
                        }
                        foreach (var item in listaBaremo)
                        {
                            try
                            {
                                var existe =  Baremos.Where(x => x.Descripcion.Equals(item.CodigoBaremo) && x.PQuinquenalId == item.PQuinquenalId).FirstOrDefault();
                                if (existe != null)
                                {
                                    Baremo obj = new Baremo();
                                    obj.Id = existe.Id;
                                    obj.CodigoBaremo = existe.Descripcion;
                                    obj.Precio = item.Precio;
                                    obj.Descripcion = item.Descripcion;
                                    obj.Estado = item.Estado;
                                    obj.PQuinquenalId = item.PQuinquenalId;
                                    listaBaremoRepetidos.Add(obj);
                                }
                                else
                                {
                                    listaBaremoInsert.Add(item);
                                }
                            }
                            catch (Exception e )
                            {

                                listaBaremoInsert.Add(item);
                            }
                            

                        }
                        if (listaBaremoInsert.Count > 0)
                        {
                          
                            _context.AddRange(listaBaremoInsert);
                            await _context.SaveChangesAsync();
                        }
                        if (listaBaremoRepetidos.Count > 0)
                        {
                            _context.Update(listaBaremoRepetidos);
                            await _context.SaveChangesAsync();
                        }


                    }
                }


                
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Baremo";
                    trazabilidad.Evento = "Importar";
                    trazabilidad.DescripcionEvento = $"Se insertó correctamente {listaBaremoInsert.Count()} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                

                dto.listaError = null;
                dto.listaRepetidos = null;
                dto.listaInsert = null;
                dto.Satisfactorios = listaBaremoInsert.Count();
                dto.Error = listaBaremoError.Count();
                dto.Actualizados = listaBaremoRepetidos.Count();
                dto.Valid = true;
                dto.Message = Constantes.SatisfactorioImport;
            }
            catch (Exception e)
            {
                dto.Satisfactorios = 0;
                dto.Error = 0;
                dto.Actualizados = 0;
                dto.Valid = false;
                dto.Message = Constantes.ErrorImport;

                return dto;
            }

            return dto;
        }

        public async Task<ResponseDTO> Editarbaremo(BaremoRequestDto data, int id, DatosUsuario usuario)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {

                var ExisteQnq = await ExisteQuinquenal(data.PlanQuinquenalId);

                if (!ExisteQnq)
                {
                    dto.Valid = false;
                    dto.Message = Constantes.NoExistePQNQ;
                    return dto;
                }
                var baremoExiste = await _context.Baremo.Where(x => x.Id == id).FirstOrDefaultAsync();

                

                if (baremoExiste != null)
                {
                    baremoExiste.Descripcion = data.Descripcion;
                    baremoExiste.Precio = data.Precio;
                    baremoExiste.CodigoBaremo = data.CodigoBaremo;
                    baremoExiste.Estado = data.Estado;
                    baremoExiste.PQuinquenalId = data.PlanQuinquenalId;
                    _context.Update(baremoExiste);
                    await _context.SaveChangesAsync();
                    dto.Message = Constantes.ActualizacionSatisfactoria;
                    dto.Valid = true;

                   
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Baremo";
                        trazabilidad.Evento = "Editar";
                        trazabilidad.DescripcionEvento = $"Se editó correctamente el Baremo {baremoExiste.CodigoBaremo} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    

                }
                else
                {
                    dto.Message = Constantes.ActualizacionError;
                    dto.Valid = true;

                }
            }
            catch (Exception)
            {

                dto.Message = Constantes.ErrorSistema;
                dto.Valid = true;
            }
            return dto;
            
        }
        public async Task<bool> ExisteQuinquenal(int data)
        {

            var existQnq = await _context.PQuinquenal.Where(x => x.Id.Equals(data)).FirstOrDefaultAsync();
            if(existQnq != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ResponseDTO> EliminarBaremo( int id, DatosUsuario usuario)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {


                var baremoExiste = await _context.Baremo.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (baremoExiste != null)
                {
                    baremoExiste.Estado = baremoExiste.Estado == true ? false : true;
                    _context.Update(baremoExiste);
                    await _context.SaveChangesAsync();
                    dto.Message = Constantes.EliminacionSatisfactoria;
                    dto.Valid = true;

                    
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Baremo";
                        trazabilidad.Evento = "Eliminar";
                        trazabilidad.DescripcionEvento = $"Se eliminó correctamente el Baremo {baremoExiste.CodigoBaremo} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    
                }
                else
                {
                    dto.Message = Constantes.ActualizacionError;
                    dto.Valid = true;

                }
            }
            catch (Exception)
            {

                dto.Message = Constantes.ErrorSistema;
                dto.Valid = true;
            }
            return dto;

        }

        public async Task<ResponseDTO> CrearBaremo(BaremoRequestDto data, DatosUsuario usuario)
        {
            ResponseDTO dto = new ResponseDTO();
            try
            {
                var baremoExiste = await _context.Baremo.Where(x => x.CodigoBaremo.Equals(data.CodigoBaremo)).FirstOrDefaultAsync();

               
                if (baremoExiste != null)
                {
                    
                    dto.Message = Constantes.ExisteRegistro;
                    dto.Valid = true;

                }
                else
                {
                    Baremo baremo = new Baremo();
                    baremo.Estado = data.Estado;
                    baremo.CodigoBaremo = data.CodigoBaremo;
                    baremo.Precio = data.Precio;
                    baremo.Descripcion = data.Descripcion;
                    baremo.PQuinquenalId = data.PlanQuinquenalId;
                    _context.Add(baremo);
                    await _context.SaveChangesAsync();
                    dto.Message = Constantes.CreacionExistosa;
                    dto.Valid = true;

                    
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Baremo";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el Baremo {baremo.CodigoBaremo} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    

                }
            }
            catch (Exception)
            {

                dto.Message = Constantes.ErrorSistema;
                dto.Valid = true;
            }
            return dto;

        }

        public async Task<PaginacionResponseDto<Baremo>> GetAll(BaremoListDto entidad)
        {

            if(entidad.Buscar != "")
            {
                int n;
                var buscar = int.TryParse(entidad.Buscar, out n);

                var queryable = _context.Baremo
                                                        .Include(x => x.PQuinquenal)
                                    .Where( x => x.CodigoBaremo.Contains(entidad.Buscar) || x.Descripcion.Contains(entidad.Buscar) ||
                                     (x.Precio.ToString()).Contains(entidad.Buscar) )
                                     .AsQueryable();


                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();


                
                int cantidad = queryable.Count();
                var objeto = new PaginacionResponseDto<Baremo>
                {
                    Cantidad = cantidad,
                    Model = entidades

                };
                return objeto;
            }
            else
            {
                var queryable = _context.Baremo
                                    .Include(x=>x.PQuinquenal)
                                    .Where(x => entidad.CodigoBaremo != "" ? x.CodigoBaremo == entidad.CodigoBaremo : true)
                                    .Where(x => entidad.Estado != false ? x.Estado == entidad.Estado : true)
                                    .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                    .Where(x => entidad.Precio > 0 ? x.Precio == entidad.Precio : true)
                                    .Where(x => entidad.PlanQuinquenalId != 0 ? x.PQuinquenalId == entidad.PlanQuinquenalId : true)
                                     .AsQueryable();


                var entidades = await queryable.OrderBy(e => e.Descripcion).Paginar(entidad)
                                       .ToListAsync();
                int cantidad = queryable.Count();
                var objeto = new PaginacionResponseDto<Baremo>
                {
                    Cantidad = cantidad,
                    Model = entidades

                };
                return objeto;
            }

            
        }
    }
}
