using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanQuinquenal.Core.Utilities;
using ApiDavis.Core.Utilidades;
using System.Data.SqlTypes;
using PlanQuinquenal.Core.DTOs;
using OfficeOpenXml;
using System.Data;
using System.ComponentModel;
using Microsoft.Data.SqlClient;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ProyectoRepositorio : IProyectoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IBaremoRepository _baremoRepository;
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public ProyectoRepositorio(PlanQuinquenalContext context, IMapper mapper, IBaremoRepository baremoRepository, IRepositoryMantenedores repositoryMantenedores)
        {
            this._context = context;
            this.mapper = mapper;
            this._baremoRepository = baremoRepository;
            this._repositoryMantenedores = repositoryMantenedores;
        }

        public async Task<ProyectoResponseDto> GetById(int id)
        {


            var existe = await _context.Proyecto.Where(x => x.Id == id).FirstOrDefaultAsync();


            var proyecto = await _context.Proyecto
                                         .Include(x => x.PlanAnual)
                                         .Include(x => x.PQuinquenal)
                                         .Include(x => x.Material)
                                         .Include(x => x.Distrito)
                                         .Include(x => x.Constructor)
                                         .Include(x => x.TipoProyecto)
                                         .Include(x => x.EstadoGeneral)
                                         .Include(x => x.TipoRegistro)
                                         .Include(x => x.Baremo)
                                         .Include(x => x.IngenieroResponsable)
                                         .Include(x => x.UsuariosInteresados)
                                         .ThenInclude(x => x.Usuario)
                                         .Where(x => x.Id == id).FirstOrDefaultAsync();

            var proyectoDto = mapper.Map<ProyectoResponseDto>(proyecto);
            return proyectoDto;

            //verificar  si no existe
        }
        public async Task<PaginacionResponseDtoException<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos)
        {

            var queryable = await _context.Proyecto
                                         .Include(x => x.PlanAnual)
                                         .Include(x => x.PQuinquenal)
                                         .Include(x => x.Material)
                                         .Include(x => x.Distrito)
                                         .Include(x => x.Constructor)
                                         .Include(x => x.TipoProyecto)
                                         .Include(x => x.EstadoGeneral)
                                         .Include(x => x.TipoRegistro)
                                         .Include(x => x.IngenieroResponsable)
                                         .Include(x => x.Baremo)
                                         .ToListAsync();

            var proyectoDto = mapper.Map<List<ProyectoResponseDto>>(queryable);



            var listaFin = proyectoDto
                                     .Where(x => filterProyectos.CodigoProyecto != "" ? x.CodigoProyecto.Contains(filterProyectos.CodigoProyecto) : true)
                                     .Where(x => filterProyectos.Etapa != 0 ? x.Etapa == filterProyectos.Etapa : true)
                                     .Where(x => filterProyectos.NombreProyecto != "" ? x.descripcion.Contains(filterProyectos.NombreProyecto) : true)
                                     .Where(x => filterProyectos.MaterialId != 0 ? x.Material.Id == filterProyectos.MaterialId : true)
                                     .Where(x => filterProyectos.DistritoId != 0 ? x.Distrito.Id == filterProyectos.DistritoId : true)
                                     .Where(x => filterProyectos.TipoProyectoId != 0 ? x.TipoProyecto.Id == filterProyectos.TipoProyectoId : true)
                                     .Where(x => filterProyectos.PQuinquenalId != 0 ? x.PQuinquenalId == filterProyectos.PQuinquenalId : true)
                                     .Where(x => filterProyectos.AñoPq != "" ? x.AñosPQ.Contains(filterProyectos.AñoPq) : true)
                                     .Where(x => filterProyectos.PAnualId != 0 ? x.PlanAnualId == filterProyectos.PAnualId : true)
                                     .Where(x => filterProyectos.CodigoMalla != "" ? x.CodigoMalla.Contains(filterProyectos.CodigoMalla) : true)
                                     .Where(x => filterProyectos.ConstructorId != 0 ? x.ConstructorId == filterProyectos.ConstructorId : true)
                                     .Where(x => filterProyectos.IngenieroId != 0 ? x.IngenieroResponsableId == filterProyectos.IngenieroId : true)
                                     .Where(x => filterProyectos.UsuarioRegisterId != 0 ? x.UsuarioRegisterId == filterProyectos.UsuarioRegisterId : true)
                                     .Where(x => filterProyectos.FechaGasificacion != "" ? x.FechaGasificacion == filterProyectos.FechaGasificacion.Replace("-", "/") : true)
                                     //.Where(x => filterProyectos.Porcentaje == 1 && filterProyectos.ValorPorcentaje != 0 ? x.Avance < Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   filterProyectos.Porcentaje == 2 && filterProyectos.ValorPorcentaje != 0 ? x.Avance <= Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   filterProyectos.Porcentaje == 3 && filterProyectos.ValorPorcentaje != 0 ? x.Avance > Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   filterProyectos.Porcentaje == 3 && filterProyectos.ValorPorcentaje != 0 ? x.Avance >= Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   filterProyectos.Porcentaje == 3 && filterProyectos.ValorPorcentaje != 0 ? x.Avance == Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   filterProyectos.Porcentaje == 3 && filterProyectos.ValorPorcentaje != 0 ? x.Avance != Convert.ToDecimal(filterProyectos.ValorPorcentaje) / 100 :
                                     //   true)
                                     .ToList();



            var fin = listaFin.Skip((filterProyectos.Pagina - 1) * filterProyectos.RecordsPorPagina).Take(filterProyectos.RecordsPorPagina);

            var objeto = new PaginacionResponseDtoException<ProyectoResponseDto>
            {
                Cantidad = listaFin.Count(),
                Model = fin
            };
            return objeto;


        }
        //public async Task<PaginacionResponseDto<ProyectoResponseDto>> GetAll(FiltersProyectos filterProyectos)
        //{

        //var queryable = _context.Proyecto
        //                             .Include(x => x.PlanAnual)
        //                             .Include(x => x.PQuinquenal)
        //                             .Include(x => x.Material)
        //                             .Include(x => x.Distrito)
        //                             .Include(x => x.Constructor)
        //                             .Include(x => x.TipoProyecto)
        //                             .Include(x => x.EstadoGeneral)
        //                             .Include(x => x.TipoRegistro)
        //                             .Include(x => x.IngenieroResponsable)
        //                             .Include(x => x.Baremo)
        //                             .Where(x => filterProyectos.CodigoProyecto != "" ? x.CodigoProyecto == filterProyectos.CodigoProyecto : true)
        //                             .Where(x => filterProyectos.Etapa != 0 ? x.Etapa == filterProyectos.Etapa : true)
        //                             .Where(x => filterProyectos.NombreProyecto != "" ? x.descripcion.Contains(filterProyectos.NombreProyecto) : true)
        //                             .Where(x => filterProyectos.MaterialId != 0 ? x.Material.Id == filterProyectos.MaterialId : true)
        //                             .Where(x => filterProyectos.DistritoId != 0 ? x.Distrito.Id == filterProyectos.DistritoId : true)
        //                             .Where(x => filterProyectos.TipoProyectoId != 0 ? x.TipoProyecto.Id == filterProyectos.TipoProyectoId : true)
        //                             .Where(x => filterProyectos.PQuinquenalId != 0 ? x.PQuinquenal.Id == filterProyectos.PQuinquenalId : true)
        //                             .Where(x => filterProyectos.AñoPq != "" ? x.AñosPQ.Contains(filterProyectos.AñoPq) : true)
        //                             .Where(x => filterProyectos.PAnualId != 0 ? x.PlanAnual.Id == filterProyectos.PAnualId : true)
        //                             .Where(x => filterProyectos.CodigoMalla != "" ? x.CodigoMalla.Contains(filterProyectos.CodigoMalla) : true)
        //                             .Where(x => filterProyectos.ConstructorId != 0 ? x.ConstructorId == filterProyectos.ConstructorId : true)
        //                             .Where(x => filterProyectos.IngenieroId != 0 ? x.IngenieroResponsable.cod_usu == filterProyectos.IngenieroId : true)
        //                             .Where(x => filterProyectos.UsuarioRegisterId != 0 ? x.UsuarioRegisterId == filterProyectos.UsuarioRegisterId : true)
        //                             .AsQueryable();
        //    int cantidad = queryable.Count();
        //    var listaPaginada = await queryable.OrderBy(e => e.descripcion).Paginar(filterProyectos).ToListAsync();
        //    var proyectoDto = mapper.Map<List<ProyectoResponseDto>>(listaPaginada);

        //    var objeto = new PaginacionResponseDto<ProyectoResponseDto>
        //    {
        //        Cantidad = cantidad,
        //        Model = proyectoDto
        //    };
        //    return objeto;


        //}

        public async Task<ResponseDTO> Add(ProyectoRequestDto proyectoRequestDto, int idUser)
        {
            try
            {
                var existe = await _context.Proyecto.Where(x => x.CodigoProyecto == proyectoRequestDto.CodigoProyecto && x.Etapa == proyectoRequestDto.Etapa).FirstOrDefaultAsync();

                if (existe != null)
                {
                    return new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.ExisteRegistro
                    };
                }
                else
                {
                    Proyecto proyecto = new Proyecto();
                    proyecto.CodigoProyecto = proyectoRequestDto.CodigoProyecto;
                    proyecto.PQuinquenalId = proyectoRequestDto.PQuinquenalId == 0 ? null : proyectoRequestDto.PQuinquenalId;
                    proyecto.AñosPQ = proyectoRequestDto.AñosPQ;
                    proyecto.PlanAnualId = proyectoRequestDto.PlanAnualId == 0 ? null : proyectoRequestDto.PlanAnualId;
                    proyecto.MaterialId = proyectoRequestDto.MaterialId == 0 ? null : proyectoRequestDto.MaterialId;
                    proyecto.ConstructorId = proyectoRequestDto.ConstructorId == 0 ? null : proyectoRequestDto.ConstructorId;
                    proyecto.TipoRegistroId = proyectoRequestDto.TipoRegistroId == 0 ? null : proyectoRequestDto.TipoRegistroId;
                    proyecto.LongAprobPa = proyectoRequestDto.LongAprobPa;
                    proyecto.TipoProyectoId = proyectoRequestDto.TipoProyectoId == 0 ? null : proyectoRequestDto.TipoProyectoId;
                    proyecto.BaremoId = proyectoRequestDto.BaremoId == 0 ? null : proyectoRequestDto.BaremoId;
                    proyecto.descripcion = "";
                    proyecto.Etapa = proyectoRequestDto.Etapa;
                    proyecto.IngenieroResponsableId = null;
                    proyecto.EstadoGeneralId = 2;
                    proyecto.UsuarioRegisterId = idUser;
                    proyecto.UsuarioModificaId = idUser;
                    proyecto.FechaGasificacion = null;
                    proyecto.FechaRegistro = DateTime.Now;
                    proyecto.fechamodifica = DateTime.Now;
                    proyecto.LongImpedimentos = 0;
                    proyecto.LongRealHab = 0;
                    proyecto.LongRealPend = 0;
                    proyecto.LongProyectos = 0;
                    proyecto.DistritoId = proyectoRequestDto.DistritoId;
                    _context.Add(proyecto);
                    await _context.SaveChangesAsync();

                    if (proyectoRequestDto.UsuariosInteresados.Count > 0)
                    {
                        List<UsuariosInteresadosPy> listPqUser = new List<UsuariosInteresadosPy>();
                        foreach (var item in proyectoRequestDto.UsuariosInteresados)
                        {
                            var existeUsu = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                            if (existeUsu != null)
                            {
                                UsuariosInteresadosPy pqUser = new UsuariosInteresadosPy();
                                pqUser.ProyectoId = proyecto.Id;
                                pqUser.UsuarioId = item;
                                pqUser.Estado = true;
                                listPqUser.Add(pqUser);
                            }

                        }
                        _context.AddRange(listPqUser);
                        await _context.SaveChangesAsync();
                    }

                    return new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.CreacionExistosa
                    };
                }
            }
            catch (Exception e )
            {

                return new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.ErrorSistema
                };
            }
        }

        public async Task<ResponseDTO> Update(ProyectoRequestUpdateDto p, int id, int idUser)
        {
            try
            {

               
                    var existe = await _context.Proyecto.Where(x => x.Id == id).FirstOrDefaultAsync();

                    if (existe != null)
                    {
                        existe.descripcion = p.Descripcion;
                        existe.PQuinquenalId = p.PQuinquenalId;
                        existe.AñosPQ = p.AñosPQ;
                        existe.PlanAnualId = p.PlanAnualId == null || p.PlanAnualId == 0 ? null : p.PlanAnualId;
                        existe.MaterialId = p.MaterialId == null || p.MaterialId == 0 ? null : p.MaterialId;
                        existe.DistritoId = p.DistritoId == null || p.DistritoId == 0 ? null : p.DistritoId;
                        existe.TipoProyectoId = p.TipoProyectoId == null || p.TipoProyectoId == 0 ? null : p.TipoProyectoId;
                        existe.Etapa = p.Etapa;
                        existe.CodigoMalla = p.CodigoMalla;
                        existe.TipoRegistroId = p.TipoRegistroId == null || p.TipoRegistroId == 0 ? null : p.TipoRegistroId;
                        existe.IngenieroResponsableId = p.IngenieroResponsableId == null || p.IngenieroResponsableId == 0 ? null : p.IngenieroResponsableId;
                        existe.ConstructorId = p.ConstructorId == null || p.ConstructorId == 0 ? null : p.ConstructorId;
                        existe.BaremoId = p.BaremoId == null || p.BaremoId == 0 ? null : p.BaremoId;
                        existe.FechaGasificacion = p.FechaGacificacion != "" ? DateTime.Parse(p.FechaGacificacion) : null;
                        existe.LongAprobPa = p.LongAprobPa;
                        existe.LongRealHab = p.LongRealHab;
                        existe.LongProyectos = p.LongProyectos;
                        existe.UsuarioModificaId = idUser;
                        existe.fechamodifica = DateTime.Now;
                        _context.Update(existe);
                        await _context.SaveChangesAsync();

                        if (p.UsuariosInteresados.Count > 0)
                        {
                            var userInt = await _context.UsuariosInteresadosPy.Where(x => x.ProyectoId == id).ToListAsync();

                            foreach (var item in userInt)
                            {
                                _context.Remove(item);
                                await _context.SaveChangesAsync();
                            }
                            List<UsuariosInteresadosPy> listPqUser = new List<UsuariosInteresadosPy>();
                            foreach (var item in p.UsuariosInteresados)
                            {
                                var existeUsu = await _context.Usuario.Where(x => x.cod_usu == item).FirstOrDefaultAsync();
                                if (existeUsu != null)
                                {
                                    UsuariosInteresadosPy pqUser = new UsuariosInteresadosPy();
                                    pqUser.ProyectoId = id;
                                    pqUser.UsuarioId = item;
                                    pqUser.Estado = true;
                                    listPqUser.Add(pqUser);
                                }
                            }
                            foreach (var item in listPqUser)
                            {
                                _context.Add(item);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var objeto = new ResponseDTO
                        {
                            Message = Constantes.ActualizacionSatisfactoria,
                            Valid = true
                        };
                        return objeto;
                    }
                    else
                    {
                        var objeto = new ResponseDTO
                        {
                            Message = Constantes.BusquedaNoExitosa,
                            Valid = true
                        };
                        return objeto;
                    }

                

            }
            catch (Exception e)
            {

                var objeto = new ResponseDTO
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false
                };
                return objeto;
            }
        }

        public async Task<PaginacionResponseDtoException<ProyectoDetalle>> GetAll2(FiltersProyectos f)
        {
            if (f.CodigoProyecto.Equals("")) f.CodigoProyecto = null;
            if (f.NroExpediente.Equals("")) f.NroExpediente = null;
            if (f.AñoPq.Equals("")) f.AñoPq = null;
            if (f.CodigoMalla.Equals("")) f.CodigoMalla = null;
            if (f.NombreProyecto.Equals("")) f.NombreProyecto = null;
            if (f.ProblematicaReal.Equals("")) f.ProblematicaReal = null;
            if (f.FechaGasificacion.Equals("")) f.FechaGasificacion = null;
            //else
            //{
            //    string[] cadena = f.FechaGasificacion.Split("-");

            //    f.FechaGasificacion = cadena[2] + "-" + cadena[1] +"-" + cadena[0];
            //}
            if (f.EstadoGeneral == 0) f.EstadoGeneral = null;
            if (f.Etapa == 0) f.Etapa = null;
            if (f.MaterialId == 0) f.MaterialId = null;
            if (f.DistritoId == 0) f.DistritoId = null;
            if (f.TipoProyectoId == 0) f.TipoProyectoId = null;
            if (f.PQuinquenalId == 0) f.PQuinquenalId = null;
            if (f.PAnualId == 0) f.PAnualId = null;
            if (f.ConstructorId == 0) f.ConstructorId = null;
            if (f.IngenieroId == 0) f.IngenieroId = null;
            if (f.UsuarioRegisterId == 0) f.UsuarioRegisterId = null;



            var resultad = await _context.ProyectoDetalle.FromSqlInterpolated($"EXEC listar {f.CodigoProyecto} , {f.NroExpediente} , {f.AñoPq} , {f.CodigoMalla} , {f.NombreProyecto} , {f.ProblematicaReal} , {f.FechaGasificacion} , {f.EstadoGeneral} , {f.Etapa} , {f.MaterialId} , {f.DistritoId} , {f.TipoProyectoId} , {f.PQuinquenalId} , {f.PAnualId} , {f.ConstructorId} , {f.IngenieroId} , {f.UsuarioRegisterId} , {f.Pagina} , {f.RecordsPorPagina}").ToListAsync();


            var dato = new PaginacionResponseDtoException<ProyectoDetalle>
            {
                Cantidad = resultad.Count() == 0 ? 0 : resultad.ElementAt(0).Total,
                Model = resultad
            };
            return dato;
        }

        public async Task<ImportResponseDto<Proyecto>> ProyectoImport(RequestMasivo data)
        {

            ImportResponseDto<Proyecto> dto = new ImportResponseDto<Proyecto>();
            var Material = await _repositoryMantenedores.GetAllByAttribute(Constantes.Material);
            var Constructor = await _repositoryMantenedores.GetAllByAttribute(Constantes.Constructor);
            var TipoProyecto = await _repositoryMantenedores.GetAllByAttribute(Constantes.TipoProyecto);
            var Distrito = await _repositoryMantenedores.GetAllByAttribute(Constantes.Distrito);
            var TipoRegistroPY = await _repositoryMantenedores.GetAllByAttribute(Constantes.TipoRegistroPY);
            var PlanQuin = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");
            var PlanAnu = await _repositoryMantenedores.GetAllByAttribute("PlanAnual");
            var Baremos = await _repositoryMantenedores.GetAllByAttribute("Baremo");
            var proyectosMasivos = await _context.ProyectoMasivoDetalle.FromSqlInterpolated($"EXEC listaMasiva").ToListAsync();

            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            List<Proyecto> lista = new List<Proyecto>();
            List<Proyecto> listaError = new List<Proyecto>();
            List<Proyecto> listaRepetidos = new List<Proyecto>();
            List<Proyecto> listaInsert = new List<Proyecto>();
            List<Proyecto> listaRepetidosInsert = new List<Proyecto>();

            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Proyecto"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var codPry = worksheet.Cells[row, 1].Value?.ToString();
                            var codPQ = worksheet.Cells[row, 2].Value?.ToString();
                            var anioPQ = worksheet.Cells[row, 3].Value?.ToString();
                            var anioPA = worksheet.Cells[row, 4].Value?.ToString();
                            var etapa = worksheet.Cells[row, 5].Value?.ToString();
                            var material = worksheet.Cells[row, 6].Value?.ToString();
                            var constructor = worksheet.Cells[row, 7].Value?.ToString();
                            var tipoRegistro = worksheet.Cells[row, 8].Value?.ToString();
                            var distrito = worksheet.Cells[row, 9].Value?.ToString();
                            var tipoProyecto = worksheet.Cells[row, 10].Value?.ToString();
                            var longAproPa = worksheet.Cells[row, 11].Value?.ToString();
                            var longRealHab = worksheet.Cells[row, 12].Value?.ToString();
                            var longRealPend = worksheet.Cells[row, 13].Value?.ToString();
                            var codMalla = worksheet.Cells[row, 14].Value?.ToString();
                            var codVNR = worksheet.Cells[row, 15].Value?.ToString();

                            var dPQ = PlanQuin.Where(x => x.Descripcion == codPQ).FirstOrDefault();
                            var PlanAnualId = PlanAnu.Where(x => x.Descripcion == anioPA).FirstOrDefault();
                            var dMaterial = Material.Where(x => x.Descripcion == material).FirstOrDefault();
                            var dConstructor = Constructor.Where(x => x.Descripcion == constructor).FirstOrDefault();
                            var dTipoProyecto = TipoProyecto.Where(x => x.Descripcion == tipoProyecto).FirstOrDefault();
                            var dDistrito = Distrito.Where(x => x.Descripcion == distrito).FirstOrDefault();
                            var dTipoRegistroPY = TipoRegistroPY.Where(x => x.Descripcion == tipoRegistro).FirstOrDefault();
                            var dCodigoVNR = Baremos.Where(x => x.Descripcion == codVNR).FirstOrDefault();
                            int codigoVNR = 0;
                            if (dCodigoVNR != null)
                            {
                                codigoVNR = dCodigoVNR.Id;
                            }


                            if (dPQ == null || codigoVNR == 0 || dMaterial == null || dConstructor == null || dTipoProyecto == null || dDistrito == null)
                            {
                                var entidadError = new Proyecto
                                {
                                    CodigoProyecto = codPry,

                                };
                                listaError.Add(entidadError);

                            }
                            else
                            {
                                try
                                {
                                    var entidad = new Proyecto
                                    {
                                        CodigoProyecto = codPry,
                                        PQuinquenalId = dPQ.Id,
                                        AñosPQ = anioPQ,
                                        PlanAnualId = PlanAnualId.Id,
                                        Etapa = Convert.ToInt32(etapa),
                                        MaterialId = dMaterial.Id,
                                        ConstructorId = dConstructor.Id,
                                        TipoRegistroId = dTipoRegistroPY.Id,
                                        DistritoId = dDistrito.Id,
                                        TipoProyectoId = dTipoProyecto.Id,
                                        LongAprobPa = Decimal.Parse(longAproPa),
                                        LongRealHab = Decimal.Parse(longRealHab),
                                        LongRealPend = Decimal.Parse(longRealPend),
                                        //LongImpedimentos = Decimal.Parse(longImpedimentos),
                                        //LongReemplazada = Decimal.Parse(longReemplazada),
                                        CodigoMalla = codMalla,
                                        BaremoId = codigoVNR

                                    };
                                    lista.Add(entidad);
                                }
                                catch (Exception e)
                                {
                                    var entidadError = new Proyecto
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
                                var existes = proyectosMasivos.Where(x => x.CodigoProyecto.Equals(item.CodigoProyecto) && x.Etapa.Equals(item.Etapa)).FirstOrDefault();

                                if (existes != null)
                                {
                                    var repetidos = new Proyecto
                                    {
                                        CodigoProyecto = existes.CodigoProyecto
                                    };
                                    listaRepetidos.Add(repetidos);
                                    Proyecto existe = new Proyecto();
                                    existe.Id = existes.Id;
                                    existe.CodigoProyecto = existes.CodigoProyecto;
                                    existe.descripcion = existes.descripcion;
                                    existe.PQuinquenalId = item.PQuinquenalId;
                                    existe.AñosPQ = item.AñosPQ;
                                    existe.PlanAnualId = item.PlanAnualId;
                                    existe.MaterialId = item.MaterialId;
                                    existe.DistritoId = item.DistritoId;
                                    existe.TipoProyectoId = item.TipoProyectoId;
                                    existe.Etapa = item.Etapa;
                                    existe.CodigoMalla = item.CodigoMalla;
                                    existe.TipoRegistroId = item.TipoRegistroId;
                                    existe.IngenieroResponsableId = existes.IngenieroResponsableId;
                                    existe.ConstructorId = item.ConstructorId;
                                    existe.EstadoGeneralId = existes.EstadoGeneralId;
                                    existe.BaremoId = item.BaremoId;
                                    existe.UsuarioRegisterId = existes.UsuarioRegisterId;
                                    existe.UsuarioModificaId = existes.UsuarioModificaId;
                                    existe.FechaRegistro = existes.FechaRegistro;
                                    existe.fechamodifica = existes.fechamodifica;
                                    existe.FechaGasificacion = existes.FechaGasificacion;
                                    existe.LongAprobPa = item.LongAprobPa;
                                    existe.LongRealHab = item.LongRealHab;
                                    existe.LongRealPend = item.LongRealPend;
                                    existe.LongImpedimentos = existes.LongImpedimentos;
                                    existe.LongReemplazada = existes.LongReemplazada;
                                    existe.LongProyectos = existes.LongProyectos;

                                    listaRepetidosInsert.Add(existe);

                                }
                                else
                                {
                                    listaInsert.Add(item);
                                }
                            }
                            catch (Exception e)
                            {

                                var entidadError = new Proyecto
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

                            //var numerosParam = new SqlParameter("@ObjectList", SqlDbType.Structured)
                            //{
                            //    TypeName = "dbo.Proyecto", // Reemplaza dbo.NumeroTableType por el nombre de tu tipo de tabla definido en SQL Server
                            //    Value = ToDataTable(listaRepetidosInsert)
                            //};
                            //var proyectosMasivoss = _context.ProyectoMasivoDetalle.FromSqlRaw($"EXEC UpdateObjectList {numerosParam}");
                            //// Crea un parámetro de tipo tabla en SQL Server

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

        public static DataTable ToDataTable(IEnumerable<Proyecto> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Proyecto));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }
            return table;
        }


    }
}
