using ApiDavis.Core.Utilidades;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        public BaremoRepository(PlanQuinquenalContext context) 
        {
            this._context = context;
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
        public async Task<ImportResponseDto<Baremo>> BaremoImport(RequestMasivo data)
        {
            ImportResponseDto<Baremo> dto = new ImportResponseDto<Baremo>();
            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<Baremo> listaBaremo = new List<Baremo>();
            List<Baremo> listaBaremoError = new List<Baremo>();
            List<Baremo> listaBaremoRepetidos = new List<Baremo>();
            List<Baremo> listaBaremoInsert = new List<Baremo>();

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
                            
                            var dato =await _context.PlanQuinquenal.Where(x => x.Pq == valor4).FirstOrDefaultAsync();
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
                                    PlanQuinquenalId = dato.Id
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
                                var existe = await _context.Baremo.Where(x => x.CodigoBaremo.Equals(item.CodigoBaremo)).FirstAsync();
                                if (existe != null)
                                {
                                    listaBaremoRepetidos.Add(item);
                                    existe.Precio = item.Precio;
                                    existe.Descripcion = item.Descripcion;
                                    existe.Estado = item.Estado;
                                    existe.PlanQuinquenalId = item.PlanQuinquenalId;
                                    _context.Baremo.Update(existe);
                                    await _context.SaveChangesAsync();
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
                            await _context.Baremo.AddRangeAsync(listaBaremoInsert);
                            await _context.SaveChangesAsync();
                        }


                    }
                }


                dto.listaError = listaBaremoError;
                dto.listaRepetidos = listaBaremoRepetidos;
                dto.listaInsert = listaBaremoInsert;
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

        public async Task<ResponseDTO> Editarbaremo(BaremoRequestDto data, int id)
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
                    baremoExiste.PlanQuinquenalId = data.PlanQuinquenalId;
                    _context.Update(baremoExiste);
                    await _context.SaveChangesAsync();
                    dto.Message = Constantes.ActualizacionSatisfactoria;
                    dto.Valid = true;

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

            var existQnq = await _context.PlanQuinquenal.Where(x => x.Id.Equals(data)).FirstOrDefaultAsync();
            if(existQnq != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ResponseDTO> EliminarBaremo( int id)
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
                    dto.Message = Constantes.ActualizacionSatisfactoria;
                    dto.Valid = true;

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

        public async Task<ResponseDTO> CrearBaremo(BaremoRequestDto data)
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
                    _context.Add(baremo);
                    await _context.SaveChangesAsync();
                    dto.Message = Constantes.CreacionExistosa;
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

        public async Task<PaginacionResponseDto<Baremo>> GetAll(BaremoListDto entidad)
        {

            if(entidad.Buscar != "")
            {
                int n;
                var buscar = int.TryParse(entidad.Buscar, out n);

                var queryable = _context.Baremo
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
                                    .Where(x => entidad.CodigoBaremo != "" ? x.CodigoBaremo == entidad.CodigoBaremo : true)
                                    .Where(x => entidad.Estado != false ? x.Estado == entidad.Estado : true)
                                    .Where(x => entidad.Descripcion != "" ? x.Descripcion == entidad.Descripcion : true)
                                    .Where(x => entidad.Precio > 0 ? x.Precio == entidad.Precio : true)
                                    .Where(x => entidad.Precio != 0 ? x.PlanQuinquenalId == entidad.PlanQuinquenalId : true)
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
