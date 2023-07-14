using ApiDavis.Core.Utilidades;
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

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ImpedimentoRepository : IImpedimentoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public ImpedimentoRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        public async Task<ResponseDTO> Add(ImpedimentoRequestDTO p,int idUser)
        {
            try
            {
                var proy = await _context.Proyecto.Where(x => x.Id == p.ProyectoId).FirstOrDefaultAsync();
                var baremo = await _context.Baremo.Where(x=>x.Id  == proy.BaremoId).FirstOrDefaultAsync();
                Impedimento obj = new Impedimento();
                obj.ProyectoId = p.ProyectoId;
                obj.ProblematicaRealId = p.ProblematicaRealId;
                obj.LongImpedimento = p.LongImpedimento;
                obj.CausalReemplazoId = null;
                obj.Resuelto = false;
                obj.PrimerEstrato = 0;
                obj.SegundoEstrato = 0;
                obj.TercerEstrato = 0;
                obj.CuartoEstrato = 0;
                obj.QuintoEstrato = 0;
                obj.LongitudReemplazo = 0;
                obj.ValidacionCargoPlano = false;
                obj.ValidacionCargoSustentoRRCC = false;
                obj.ValidacionCargoSustentoAmbiental = false;
                obj.ValidacionCargoSustentoArqueologia = false;
                obj.ValidacionLegalId = null;
                obj.Comentario = "";
                obj.FechaPresentacion = null;
                obj.FechaPresentacionReemplazo = null;
                obj.fechamodifica = DateTime.Now;
                obj.FechaRegistro = DateTime.Now;
                obj.UsuarioRegisterId = idUser;
                obj.UsuarioModificaId = idUser;
                obj.CostoInversion = baremo.Precio * p.LongImpedimento;


                _context.Add(obj);
                await _context.SaveChangesAsync();
                var dato = new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
                return dato;
            }
            catch (Exception)
            {
                var dato = new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
                return dato;
                throw;
            }

            

        }
        public bool saveDocument(ImpedimentoDocumentoDto documentoRequestDto, Guid guidId)
        {
            try
            {
                string rutaCompleta = "";
                string ruta = "";
                string modulo = "Impedimentos";
                string gestion = documentoRequestDto.Gestion;
                ruta = configuration["RUTA_ARCHIVOS"] + $"\\{modulo + "\\"}";

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }


                rutaCompleta = ruta + documentoRequestDto.CodigoImpedimento + "\\" + gestion;



                if (!Directory.Exists(rutaCompleta))
                {
                    Directory.CreateDirectory(rutaCompleta);
                }
                string rutaSave = Path.Combine(rutaCompleta, guidId + Path.GetExtension(documentoRequestDto.NombreDocumento));

                byte[] decodedBytes = Convert.FromBase64String(documentoRequestDto.base64);
                File.WriteAllBytes(rutaSave, decodedBytes);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public async Task<ResponseDTO> Documentos(ImpedimentoDocumentoDto p, int idUser)
        {

            var resultado = await _context.Impedimento.Where(x => x.Id == p.CodigoImpedimento).FirstOrDefaultAsync();
            if (resultado == null)
            {
                var resps = new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.BusquedaNoExitosa
                };
                return resps;
            }
            else
            {


                DocumentosImpedimento d = new DocumentosImpedimento();
                var guidId = Guid.NewGuid();
                d.ImpedimentoId = p.CodigoImpedimento;
                d.CodigoDocumento = guidId.ToString();
                d.NombreDocumento = p.NombreDocumento;
                d.Gestion = p.Gestion;
                d.FechaRegistro = DateTime.Now;
                d.TipoDocumento = Path.GetExtension(p.NombreDocumento);
                d.rutaFisica = configuration["RUTA_ARCHIVOS"] + "\\" + "Impedimentos\\"  + p.CodigoImpedimento + "\\" + p.Gestion + "\\" + guidId + Path.GetExtension(p.NombreDocumento);
                d.ruta = configuration["DNS"] + "Impedimentos" + "/" + p.CodigoImpedimento + "/" + p.Gestion + "/" + guidId + Path.GetExtension(p.NombreDocumento);
                d.Estado = true;
                d.UsuarioRegister = idUser;
                _context.Add(d);
                await _context.SaveChangesAsync();

                saveDocument(p, guidId);
                var resp = new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
                return resp;
            }
            
        }

        public async Task<ResponseEntidadDto<ImpedimentoDetalle>> GetById(int Id)
        {
            var resultad = await _context.ImpedimentoDetalle.FromSqlRaw($"EXEC impedimentobyid  {Id}").ToListAsync();

            if (resultad.Count > 0)
            {
                var result = new ResponseEntidadDto<ImpedimentoDetalle>
                {
                    Message = Constantes.BusquedaExitosa,
                    Valid = true,
                    Model = resultad[0]
                };
                return result;
            }
            else
            {
                var result = new ResponseEntidadDto<ImpedimentoDetalle>
                {
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false,
                    Model = null
                };
                return result;
            }
        }

        public async Task<ResponseDTO> Update(ImpedimentoUpdateDto p, int idUser, int id)
        {
            try
            {
                var existe = await _context.Impedimento.Where(x => x.Id == id).FirstOrDefaultAsync();
                if(existe == null)
                {
                    var re = new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.BusquedaNoExitosa
                    };
                    return re;
                }
                else
                {
                    existe.ProyectoId = p.ProyectoId;
                    existe.ProblematicaRealId = p.ProblematicaRealId==0?null:p.ProblematicaRealId;
                    existe.LongImpedimento = p.LongImpedimento==null?0:p.LongImpedimento;
                    existe.CausalReemplazoId = p.CausalReemplazoId==0 || p.CausalReemplazoId == null?null:p.CausalReemplazoId;
                    existe.Resuelto = false;
                    existe.PrimerEstrato = p.PrimerEstrato==0?0:p.PrimerEstrato;
                    existe.SegundoEstrato = p.SegundoEstrato == 0 ? 0 : p.SegundoEstrato;
                    existe.TercerEstrato = p.TercerEstrato == 0 ? 0 : p.TercerEstrato;
                    existe.CuartoEstrato = p.CuartoEstrato == 0 ? 0 : p.CuartoEstrato;
                    existe.QuintoEstrato = p.QuintoEstrato == 0 ? 0 : p.QuintoEstrato;
                    existe.LongitudReemplazo = p.LongitudReemplazo==null?0:p.LongitudReemplazo;
                    existe.ValidacionCargoPlano = p.ValidacionCargoPlano==null?false:p.ValidacionCargoPlano;
                    existe.ValidacionCargoSustentoRRCC = p.ValidacionCargoSustentoRRCC == null ? false : p.ValidacionCargoSustentoRRCC;
                    existe.ValidacionCargoSustentoAmbiental = p.ValidacionCargoSustentoAmbiental == null ? false : p.ValidacionCargoSustentoAmbiental;
                    existe.ValidacionCargoSustentoArqueologia = p.ValidacionCargoSustentoArqueologia == null ? false : p.ValidacionCargoSustentoArqueologia;
                    existe.ValidacionLegalId = p.ValidacionLegalId == null || p.ValidacionLegalId==0? null : p.ValidacionLegalId;
                    existe.FechaPresentacion = p.FechaPresentacion==null ?null:p.FechaPresentacion;
                    existe.Comentario = p.Comentario == null?"":p.Comentario;
                    existe.FechaPresentacionReemplazo = p.FechaPresentacionReemplazo == null ? null : p.FechaPresentacionReemplazo;
                    existe.fechamodifica = DateTime.Now;
                    existe.UsuarioModificaId = idUser;
                    existe.CostoInversion = p.CostoInversion == null ? 0 : p.CostoInversion;

                    _context.Update(existe);
                    await _context.SaveChangesAsync();

                    var resultado = new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.ActualizacionSatisfactoria
                    };
                    return resultado;
                }
                
            }
            catch (Exception)
            {

                var resultado = new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
                return resultado;
            }
        }

        public async Task<PaginacionResponseDtoException<Object>> ListarDocumentos(ListaDocImpedimentosDTO p)
        {
            var queryable =  _context.DocumentosImpedimento.Where(x => x.ImpedimentoId == p.CodigoImpedimento && x.Gestion == p.Gestion).OrderBy(x=>x.FechaRegistro).AsQueryable();

            var entidades = await queryable.Paginar(p)
                                      .ToListAsync();

            int cantidad = queryable.Count();
            var objeto = new PaginacionResponseDtoException<Object>
            {
                Cantidad = cantidad,
                Model = entidades
            };

            return objeto;
        }

        public async Task<PaginacionResponseDto<ImpedimentoDetalle>> Listar(ImpedimentoRequestListDto p)
        {

            throw new NotImplementedException();
        }
    }
}
