using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly PlanQuinquenalContext _context;
        private readonly Constantes _constante;

        public UsuarioController(IUsuarioRepository usuarioRepository, PlanQuinquenalContext context,Constantes constante)
        {
            this.usuarioRepository = usuarioRepository;
            this._context = context;
            this._constante = constante;
        }

        [HttpGet]
        public async Task<IActionResult> Get(UsuarioListDTO usuarioListDTO)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> GetAll(UsuarioListDTO usuarioListDTO)
        {
            var resultado = await usuarioRepository.GetAll(usuarioListDTO);
            return Ok();
        }

        [HttpPost("BaremoImport")]
        public async Task<IActionResult> BaremoImport(ProyectoRequest data)
        {
            ImportResponseDto dto = new ImportResponseDto();
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
                            try
                            {
                                var entidad = new Baremo
                                {
                                    CodigoBaremo = valor1,
                                    Descripcion = valor2,
                                    Precio = Convert.ToDecimal(valor3)
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
                            var existe = await _context.Baremo.Where(x => x.CodigoBaremo.Equals(item.CodigoBaremo)).FirstAsync();
                            if(existe != null)
                            {
                                listaBaremoRepetidos.Add(item);
                                existe.Precio = item.Precio;
                                _context.Baremo.Update(existe);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                listaBaremoInsert.Add(item);
                                
                                   
                                
                            }

                        }
                        if(listaBaremoInsert.Count > 0)
                        {
                            await _context.Baremo.AddRangeAsync(listaBaremoInsert);
                            await _context.SaveChangesAsync();
                        }
                       
                        
                    }
                }


                dto.listaBaremoError= listaBaremoError;
                dto.listaBaremoRepetidos = listaBaremoRepetidos;
                dto.listaBaremoInsert = listaBaremoInsert;
                dto.Satisfactorios = listaBaremoInsert.Count();
                dto.Error = listaBaremoError.Count();
                dto.Actualizados = listaBaremoRepetidos.Count();
                dto.Valid = true;
                dto.Message = Constantes.SatisfactorioImport;
            }
            catch (Exception e )
            {
                dto.Satisfactorios = 0;
                dto.Error = 0;
                dto.Actualizados = 0;
                dto.Valid = false;
                dto.Message = Constantes.ErrorImport;

                return Ok(dto);
            }
           
            return Ok(dto);
            
             
        }
    }
}
