using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
    public class BolsaReemplazoRepository : IBolsaReemplazoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public BolsaReemplazoRepository(PlanQuinquenalContext context, IMapper mapper, IRepositoryMantenedores repositoryMantenedores)
        {
            this._context = context;
            this.mapper = mapper;
            this._repositoryMantenedores = repositoryMantenedores;
        }

        public async Task<ResponseDTO> Update(RequestUpdateBolsaDTO p, int id, int idUser)
        {
            var br = await _context.BolsaReemplazo.Where(x => x.Id != id && x.CodigoProyecto.Equals(p.CodigoProyecto)).FirstOrDefaultAsync();
            if (br == null)
            {
                var brUpdate = await _context.BolsaReemplazo.Where(x => x.Id == id).FirstOrDefaultAsync();
                brUpdate.CodigoProyecto = p.CodigoProyecto;
                brUpdate.DistritoId = p.DistritoId == 0 ? null : p.DistritoId;
                brUpdate.ConstructorId = p.ConstructorId == 0 ? null : p.ConstructorId;
                brUpdate.CodigoMalla = p.CodigoMalla;
                brUpdate.Estrato1 = p.Estrato1;
                brUpdate.Estrato2 = p.Estrato2;
                brUpdate.Estrato3 = p.Estrato3;
                brUpdate.Estrato4 = p.Estrato4;
                brUpdate.Estrato5 = p.Estrato5;
                brUpdate.CostoInversion = p.CostoInversion;
                brUpdate.LongitudReemplazo = p.LongitudReemplazo;
                brUpdate.ReemplazoId = p.ReemplazoId == 0 ? null: p.ReemplazoId;
                brUpdate.PermisoId = p.PermisoId==0 ?null : p.PermisoId;
                brUpdate.UsuarioModifica = idUser;
                brUpdate.FechaModifica = DateTime.Now;
                _context.Update(brUpdate);
                await _context.SaveChangesAsync();

                var result = new ResponseDTO
                {
                    Message = Constantes.ActualizacionSatisfactoria,
                    Valid = true
                };
                return result;
            }
            else
            {
                var result = new ResponseDTO
                {
                    Message = Constantes.ExisteRegistro,
                    Valid = false
                };
                return result;
            }
        }
        public async Task<ResponseDTO> Add(RequestBolsaDto p, int idUser)
        {
            try
            {
                var existe = await _context.BolsaReemplazo.Where(x => x.CodigoProyecto == p.CodigoProyecto).FirstOrDefaultAsync();

                if(existe== null)
                {
                    var map = mapper.Map<BolsaReemplazo>(p);
                    map.FechaModifica = DateTime.Now;
                    map.FechaRegistro = DateTime.Now;
                    map.UsuarioModifica = idUser;
                    map.UsuarioRegistra = idUser;
                    map.ConstructorId = p.ContratistaId;
                    map.Estado = true;
                    map.Estrato1 = 0;
                    map.Estrato2 = 0;
                    map.Estrato3 = 0;
                    map.Estrato4 = 0;
                    map.Estrato5 = 0;
                    map.CostoInversion = 0;
                    map.LongitudReemplazo = 0;

                    _context.Add(map);
                    await _context.SaveChangesAsync();
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
                        Message = Constantes.ExisteRegistro
                    };
                }
            }
            catch (Exception e)
            {

                return new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
            }

        }
        public async Task<PaginacionResponseDtoException<BolsaDetalle>> Listar(BolsaRequestList f)
        {
            
            //string
            if (f.CodigoProyecto.Equals("")) f.CodigoProyecto = null;
            if (f.CodigoMalla.Equals("")) f.CodigoMalla = null;
            if (f.RiesgoSocial.Equals("")) f.RiesgoSocial = null;
            if (f.FechaRegistro.Equals("")) f.FechaRegistro = null;
            if (f.FechaModifica.Equals("")) f.FechaModifica = null;
            //int

            if (f.DistritoId == 0) f.DistritoId = null;
            if (f.ConstructorId == 0) f.ConstructorId = null;
            if (f.PermisoId == 0) f.PermisoId = null;
            if (f.Estrato1 == 0) f.Estrato1 = null;
            if (f.Estrato2 == 0) f.Estrato2 = null;
            if (f.Estrato3 == 0) f.Estrato3 = null;
            if (f.Estrato4 == 0) f.Estrato4 = null;
            if (f.Estrato5 == 0) f.Estrato5 = null;
            if (f.CostoInversion == 0) f.CostoInversion = null;
            if (f.LongitudReemplazo == 0) f.LongitudReemplazo = null;
            if (f.ReemplazoId == 0) f.ReemplazoId = null;
            if (f.UsuarioRegistro == 0) f.UsuarioRegistro = null;
            if (f.UsuarioModifica == 0) f.UsuarioModifica = null;

            var resultad = await _context.BolsaDetalle.FromSqlInterpolated($"EXEC LISTARREEMPLAZO {f.CodigoProyecto} , {f.DistritoId} , {f.ConstructorId} , {f.CodigoMalla} , {f.PermisoId} , {f.Estrato1} , {f.Estrato2} , {f.Estrato3} , {f.Estrato4} , {f.Estrato5} , {f.CostoInversion}, {f.LongitudReemplazo} , {f.RiesgoSocial} , {f.ReemplazoId} , {f.FechaRegistro} , {f.FechaModifica} , {f.UsuarioRegistro} , {f.UsuarioModifica} , {f.Pagina} , {f.RecordsPorPagina}").ToListAsync();

            var dato = new PaginacionResponseDtoException<BolsaDetalle>
            {
                Cantidad = resultad.Count() == 0 ? 0 : resultad.ElementAt(0).Total,
                Model = resultad
            };
            return dato;
        }

        public async Task<ImportResponseDto<BolsaReemplazo>> ImportarMasivo(RequestMasivo data, int idUser)
        {
            ImportResponseDto<BolsaReemplazo> dto = new ImportResponseDto<BolsaReemplazo>();
            
            var bolsaMasivos = await _context.BolsaReemplazo.FromSqlInterpolated($"EXEC LISTARMASIVOBOLSA").ToListAsync();
            var Distritos = await _repositoryMantenedores.GetAllByAttribute(Constantes.Distrito);
            var Constructores = await _repositoryMantenedores.GetAllByAttribute(Constantes.Constructor);
            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            List<BolsaReemplazo> lista = new List<BolsaReemplazo>();
            List<BolsaReemplazo> listaError = new List<BolsaReemplazo>();
            List<BolsaReemplazo> listaRepetidos = new List<BolsaReemplazo>();
            List<BolsaReemplazo> listaInsert = new List<BolsaReemplazo>();
            List<BolsaReemplazo> listaRepetidosInsert = new List<BolsaReemplazo>();

            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["BolsaReemplazo"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var codPry = worksheet.Cells[row, 1].Value?.ToString();
                            var distrito = worksheet.Cells[row, 2].Value?.ToString();
                            var contratista = worksheet.Cells[row, 3].Value?.ToString();
                            var codigoMalla = worksheet.Cells[row, 4].Value?.ToString();


                            var proyecto = bolsaMasivos.Where(x => x.CodigoProyecto.Equals(codPry)).FirstOrDefault();
                            var distritodesc = Distritos.Where(x => x.Descripcion.Equals(distrito)).FirstOrDefault();
                            var contratistadesc = Constructores.Where(x => x.Descripcion.Equals(contratista)).FirstOrDefault();


                            if ( distritodesc == null || contratistadesc == null )
                            {
                                var entidadError = new BolsaReemplazo
                                {
                                    CodigoProyecto = codPry,

                                };
                                listaError.Add(entidadError);

                            }
                            else
                            {
                                try
                                {
                                    var entidad = new BolsaReemplazo
                                    {
                                        CodigoProyecto = codPry,
                                        DistritoId = distritodesc.Id,
                                        ConstructorId = contratistadesc.Id,
                                        CodigoMalla = codigoMalla,
                                        FechaModifica = DateTime.Now,
                                        FechaRegistro = DateTime.Now,
                                        UsuarioModifica = idUser,
                                        UsuarioRegistra = idUser,
                                        Estado = true,
                                        Estrato1 = 0,
                                        Estrato2 = 0,
                                        Estrato3 = 0,
                                        Estrato4 = 0,
                                        Estrato5 = 0,
                                        CostoInversion = 0,
                                        LongitudReemplazo = 0,


                                };
                                    lista.Add(entidad);
                                }
                                catch (Exception e)
                                {
                                    var entidadError = new BolsaReemplazo
                                    {
                                        CodigoProyecto = codPry,

                                    };
                                    listaError.Add(entidadError);

                                }

                            }


                        }
                        foreach (var item in lista)
                        {
                            try
                            {
                                var existes = bolsaMasivos.Where(x => x.CodigoProyecto.Equals(item.CodigoProyecto)).FirstOrDefault();

                                if (existes != null)
                                {
                                    var repetidos = new BolsaReemplazo
                                    {
                                        CodigoProyecto = existes.CodigoProyecto
                                    };
                                    listaRepetidos.Add(repetidos);
                                    var existe = new BolsaReemplazo
                                    {
                                        Id = existes.Id,
                                        CodigoProyecto = item.CodigoProyecto,
                                        DistritoId = item.DistritoId,
                                        ConstructorId = item.ConstructorId,
                                        CodigoMalla = item.CodigoMalla,
                                        FechaModifica = existes.FechaModifica,
                                        FechaRegistro = existes.FechaModifica,
                                        UsuarioModifica = existes.UsuarioModifica,
                                        UsuarioRegistra = existes.UsuarioRegistra,
                                        Estado = existes.Estado,
                                        Estrato1 = existes.Estrato1,
                                        Estrato2 = existes.Estrato2,
                                        Estrato3 = existes.Estrato3,
                                        Estrato4 = existes.Estrato4,
                                        Estrato5 = existes.Estrato5,
                                        CostoInversion = existes.CostoInversion,
                                        LongitudReemplazo = existes.LongitudReemplazo,
                                    };
                                    

                                    listaRepetidosInsert.Add(existe);

                                }
                                else
                                {
                                    listaInsert.Add(item);
                                }
                            }
                            catch (Exception e)
                            {

                                var entidadError = new BolsaReemplazo
                                {
                                    CodigoProyecto = item.CodigoProyecto
                                };
                                listaError.Add(entidadError);
                            }


                        }
                        if (listaInsert.Count > 0)
                        {
                            await _context.BulkInsertAsync(listaInsert);
                            await _context.SaveChangesAsync();

                        }
                        if (listaRepetidosInsert.Count > 0)
                        {
                            _context.BulkUpdate(listaRepetidosInsert);
                        }


                    }
                }


                dto.listaError = listaError;
                dto.listaRepetidos = listaRepetidos;
                dto.listaInsert = listaInsert;
                dto.Satisfactorios = listaInsert.Count();
                dto.Error = listaError.Count();
                dto.Actualizados = listaRepetidos.Count();
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

        public async Task<ResponseEntidadDto<BolsaDetalleById>> GetById(int Id)
        {
            try
            {

                var resultad = await _context.BolsaDetalleById.FromSqlRaw($"EXEC bolsaId  {Id}").ToListAsync();


                if (resultad.Count > 0)
                {
                    var result = new ResponseEntidadDto<BolsaDetalleById>
                    {
                        Message = Constantes.BusquedaExitosa,
                        Valid = true,
                        Model = resultad[0]
                    };
                    return result;
                }
                else
                {
                    var result = new ResponseEntidadDto<BolsaDetalleById>
                    {
                        Message = Constantes.BusquedaNoExitosa,
                        Valid = false,
                        Model = null
                    };
                    return result;
                }





            }
            catch (Exception e)
            {

                var result = new ResponseEntidadDto<BolsaDetalleById>
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false,
                    Model = null
                };
                return result;
            }
        }

        public async Task<ResponseDTO> GestionReemplazo(GestionReemplazoDto p, int idUser)
        {
            return new ResponseDTO
            {
                Valid = true,
                Message = "Gestión de reemplazo existosa"
            };
        }
    }
}
