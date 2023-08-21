﻿using ApiDavis.Core.Utilidades;
using AutoMapper;
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ImpedimentoRepository : IImpedimentoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IRepositoryMantenedores _repositoryMantenedores;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public ImpedimentoRepository(PlanQuinquenalContext context, IMapper mapper, IConfiguration configuration, IRepositoryMantenedores repositoryMantenedores, ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones)
        {
            this._context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this._repositoryMantenedores = repositoryMantenedores;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }
        public async Task<ResponseDTO> Add(ImpedimentoRequestDTO p, DatosUsuario usuario)
        {
            var UsuarioReg = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
            string nomPerfil = UsuarioReg[0].Perfil.nombre_perfil;
            string NomCompleto = UsuarioReg[0].nombre_usu.ToString() + " " + UsuarioReg[0].apellido_usu.ToString();
            try
            {
                var proyect = await _context.Proyecto.Where(x =>x.CodigoProyecto.Equals(p.codProyecto)).ToListAsync();
                foreach (var proy in proyect)
                {
                    Impedimento obj = new Impedimento();
                    obj.ProyectoId = proy.Id;
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
                    obj.UsuarioRegisterId = usuario.UsuaroId;
                    obj.UsuarioModificaId = usuario.UsuaroId;
                    obj.CostoInversion = proy.InversionEjecutada;
                    obj.Reemplazado = false;
                    obj.estado = true;
                    _context.Add(obj);
                    await _context.SaveChangesAsync();
                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Impedimento";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó el impedimento del proyecto {proy.CodigoProyecto} correctamente ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                    #region Comparacion de estructuras y agregacion de cambios
                    var proyecto = await _context.Proyecto.Where(x => x.Id == obj.ProyectoId).FirstOrDefaultAsync();
                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = proyecto.CodigoProyecto + " - " + obj.Id.ToString()
                    };

                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.estado_user == "A").ToListAsync();
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.regImp == true).ToListAsync();
                    foreach (var listaUsuInters in UsuarioInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        string correo = listaUsuInters.correo_usu;
                        var permiso = lstpermisos.Where(x => x.cod_usu == cod_usu);
                        if (permiso.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "IMPEDIMENTOS";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = proyecto.CodigoProyecto + " - " + obj.Id.ToString();
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el impedimento {proyecto.CodigoProyecto + " - " + obj.Id}";
                            notifProyecto.codigo = obj.Id;
                            notifProyecto.modulo = "IMP";

                            await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            await _repositoryNotificaciones.EnvioCorreoNotif(composCorreo, correo, "C", "Impedimentos");
                        }
                    }

                    #endregion
                }


                var dato = new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
                return dato;
            }
            catch (Exception ex)
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

        public async Task<ResponseDTO> Documentos(ImpedimentoDocumentoDto p, DatosUsuario usuario)
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
                d.UsuarioRegister = usuario.UsuaroId;
                _context.Add(d);
                await _context.SaveChangesAsync();


                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , CrearDocumento").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Impedimento";
                    trazabilidad.Evento = "CrearDocumento";
                    trazabilidad.DescripcionEvento = $"Se creó correctamente el documento {d.CodigoDocumento} en Impedimentos";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }
                saveDocument(p, guidId);
                var resp = new ResponseDTO
                {
                    Valid = true,
                    Message = Constantes.CreacionExistosa
                };
                return resp;
            }
            
        }

        public async Task<ResponseEntidadDto<ImpedimentoDetalleById>> GetById(int Id)
        {
            var resultad = await _context.ImpedimentoDetalleById.FromSqlRaw($"EXEC impedimentobyid  {Id}").ToListAsync();

            if (resultad.Count > 0)
            {
                var result = new ResponseEntidadDto<ImpedimentoDetalleById>
                {
                    Message = Constantes.BusquedaExitosa,
                    Valid = true,
                    Model = resultad[0]
                };
                return result;
            }
            else
            {
                var result = new ResponseEntidadDto<ImpedimentoDetalleById>
                {
                    Message = Constantes.BusquedaNoExitosa,
                    Valid = false,
                    Model = null
                };
                return result;
            }
        }

        public async Task<ResponseDTO> Update(ImpedimentoUpdateDto p, DatosUsuario usuario, int id)
        {
            try
            {
                var UsuarioReg = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                string nomPerfil = UsuarioReg[0].Perfil.nombre_perfil;
                string NomCompleto = UsuarioReg[0].nombre_usu.ToString() + " " + UsuarioReg[0].apellido_usu.ToString();

                var existe = await _context.Impedimento.Where(x => x.Id == id).FirstOrDefaultAsync();
                var impAnterior = mapper.Map<ImpedimentoUpdateDto>(existe);
                if (existe == null)
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
                    List<CorreoTabla> camposModificados;

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
                    existe.UsuarioModificaId = usuario.UsuaroId;
                    existe.CostoInversion = p.CostoInversion == null ? 0 : p.CostoInversion;

                    var proyecto = await _context.Proyecto.Where(x => x.Id == existe.ProyectoId).FirstOrDefaultAsync();
                    camposModificados = CompararPropiedadesAsync(proyecto.CodigoProyecto,existe, NomCompleto).GetAwaiter().GetResult();


                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , Editar").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Impedimento";
                        trazabilidad.Evento = "Editar";
                        trazabilidad.DescripcionEvento = $"Se actualizó el impedimento {existe.Id} correctamente  ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                    _context.Update(existe);
                    await _context.SaveChangesAsync();

                    #region Notificacion
                    if (camposModificados.Count>0)
                    {
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.modImp == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).ToListAsync();
                        if (UsuarioInt!=null && proyecto!=null)
                        {
                            lstpermisos.ForEach(async per => {
                                string correo = UsuarioInt.Find(usu => usu.cod_usu == per.cod_usu).correo_usu.ToString();
                                Notificaciones notifProyecto = new Notificaciones();
                                notifProyecto.cod_usu = per.cod_usu;
                                notifProyecto.seccion = "IMPEDIMENTO";
                                notifProyecto.nombreComp_usu = NomCompleto;
                                notifProyecto.cod_reg = proyecto.CodigoProyecto + " - "+ existe.Id;
                                notifProyecto.area = nomPerfil;
                                notifProyecto.fechora_not = DateTime.Now;
                                notifProyecto.flag_visto = false;
                                notifProyecto.tipo_accion = "M";
                                notifProyecto.mensaje = $"Se modificó el Impedimento {proyecto.CodigoProyecto}";
                                notifProyecto.codigo = existe.Id;
                                notifProyecto.modulo = "IMP";

                                var respuestNotif = await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                                dynamic objetoNotif = JsonConvert.DeserializeObject(respuestNotif.ToString());
                                int codigoNotifCreada = int.Parse(objetoNotif.codigoNot.ToString());
                                await _repositoryNotificaciones.EnvioCorreoNotif(camposModificados, correo, "M", "Impedimento");
                                camposModificados.ForEach(item => {
                                    item.idNotif = codigoNotifCreada;
                                    item.id = null;
                                    });
                                _context.CorreoTabla.AddRange(camposModificados);
                                _context.SaveChanges();
                            });
                        
                        }
                    }
                    #endregion

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

        public async Task<List<CorreoTabla>> CompararPropiedadesAsync(string codigo, object valOriginal, string nomCompleto)
        {
            List<CorreoTabla> camposModificados = new List<CorreoTabla>();
            if (valOriginal!= null)
            {
                DateTime fechaActual = DateTime.Today;
                string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
                var compara = _context.Entry(valOriginal).Properties.Where(p=>p.IsModified).ToList();

                foreach (var item in compara)
                {
                    if (item.Metadata.Name.Equals("fechamodifica") || item.Metadata.Name.Equals("UsuarioModificaId"))
                    {
                        continue;
                    }
                    else if (item.Metadata.Name.Equals("ProblematicaRealId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("ProblematicaReal");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Problematica Real");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("CausalReemplazoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("CausalReemplazo");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Causal Reemplazo");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("ValidacionLegalId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("ValidacionLegal");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Causal Reemplazo");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("FechaPresentacion"))
                    {
                        CorreoTabla fila = CrearCorreoTablaManualFecha(codigo,  item, nomCompleto, fechaFormateada, "Fecha Presentación");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("FechaPresentacionReemplazo"))
                    {
                        CorreoTabla fila = CrearCorreoTablaManualFecha(codigo,  item, nomCompleto, fechaFormateada, "Fecha Presentación Reemplazo");
                        camposModificados.Add(fila);
                    }
                    else
                    {
                        CorreoTabla fila = CrearCorreoTablaDirecto(codigo, nomCompleto, fechaFormateada, item, SepararPalabrasConMayusculas(item.Metadata.Name));
                        camposModificados.Add(fila);
                    }
                }
            }

            return camposModificados;
        }

        private string SepararPalabrasConMayusculas(string input)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];
                if (char.IsUpper(currentChar) && i > 0)
                {
                    result.Append(' ');
                }
                result.Append(currentChar);
            }

            return result.ToString().Replace("ion","ión");
        }

        private static CorreoTabla CrearCorreoTablaManualFecha(string codigo,PropertyEntry? item, string nomCompleto, string fechaFormateada,string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = codigo,
                valorActual = item.OriginalValue!= null ? ((DateTime)item.OriginalValue).ToString("dd/MM/yyyy"): "",
                campoModificado = nombreColumna,
                valorModificado = item.CurrentValue!= null ?((DateTime)item.CurrentValue).ToString("dd/MM/yyyy"): "",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTablaManual(string codigo,string valorActual,string valorModificado, string nomCompleto, string fechaFormateada,string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = codigo,
                valorActual = valorActual,
                campoModificado = nombreColumna,
                valorModificado = valorModificado,
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTabla(string valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, IEnumerable<MaestroResponseDto> enumerable, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = valOriginal,
                valorActual = item.OriginalValue!= null ? enumerable.Where(x => x.Id == (int)item.OriginalValue).FirstOrDefault().Descripcion: "",
                campoModificado = nombreColumna,
                valorModificado =item.CurrentValue!=null? enumerable.Where(x => x.Id == (int)item.CurrentValue).FirstOrDefault().Descripcion: "",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        private static CorreoTabla CrearCorreoTablaDirecto(string valOriginal, string nomCompleto, string fechaFormateada, PropertyEntry? item, string nombreColumna)
        {
            return new CorreoTabla
            {
                codigo = valOriginal,
                valorActual =item.OriginalValue != null ? (item.OriginalValue is bool?( (bool)item.OriginalValue? "Marcado" : "Desmarcado"):  item.OriginalValue +"" ):"",
                campoModificado = nombreColumna,
                valorModificado = item.CurrentValue != null ? (item.CurrentValue is bool?( (bool)item.CurrentValue? "Marcado" : "Desmarcado"): item.CurrentValue +"" ):"",
                fechaMod = fechaFormateada,
                usuModif = nomCompleto,

            };
        }

        public async Task<PaginacionResponseDtoException<Object>> ListarDocumentos(ListaDocImpedimentosDTO p)
        {
            var queryable =  _context.DocumentosImpedimento.Where(x => x.ImpedimentoId == p.CodigoImpedimento && x.Gestion == p.Gestion)
                                                            .Where(x=>x.Estado ==true)
                                                            .Where(x=> x.NombreDocumento.Contains(p.NombreDocumento))
                .OrderByDescending(x=>x.FechaRegistro).AsQueryable();

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

        public async Task<PaginacionResponseDtoException<ImpedimentoDetalle>> Listar(ImpedimentoListDto f)
        {

            if (f.CodigoProyecto.Equals("")) f.CodigoProyecto = null;
            if (f.CodigoMalla.Equals("")) f.CodigoMalla = null;
            if (f.FechaRegistro.Equals("")) f.FechaRegistro = null;

            
         
            if (f.DistritoId == 0) f.DistritoId = null;
            if (f.CausalReemplazoId == 0) f.CausalReemplazoId = null;
            if (f.ConstructorId == 0) f.ConstructorId = null;
            if (f.IngenieroResponsableId == 0) f.IngenieroResponsableId = null;
            if (f.ProblematicaRealId == 0) f.ProblematicaRealId = null;
            if (f.LongitudReemplazo == 0) f.LongitudReemplazo = null;
            if (f.LongImpedimento == 0) f.LongImpedimento = null;
            if (f.CostoInversion == 0) f.CostoInversion = null;
            if (f.PrimerEstrato == 0) f.PrimerEstrato = null;
            if (f.SegundoEstrato == 0) f.SegundoEstrato = null;
            if (f.TercerEstrato == 0) f.TercerEstrato = null;
            if (f.CuartoEstrato == 0) f.CuartoEstrato = null;
            if (f.QuintoEstrato == 0) f.QuintoEstrato = null;
            var resultad = await _context.ImpedimentoDetalle.FromSqlInterpolated($"EXEC listarimpedimento  {f.CodigoProyecto}  , {f.CodigoMalla} , {f.DistritoId} , {f.CausalReemplazoId} , {f.ConstructorId} , {f.IngenieroResponsableId} , {f.ProblematicaRealId} , {f.LongitudReemplazo} , {f.LongImpedimento} , {f.CostoInversion} , {f.PrimerEstrato} , {f.SegundoEstrato} , {f.TercerEstrato} , {f.CuartoEstrato} , {f.QuintoEstrato} , {f.FechaRegistro} , {f.Pagina} , {f.RecordsPorPagina}").ToListAsync();


            var dato = new PaginacionResponseDtoException<ImpedimentoDetalle>
            {
                Cantidad = resultad.Count() == 0 ? 0 : resultad.ElementAt(0).Total,
                Model = resultad
            };
            return dato;
        }

        public async Task<DocumentoResponseDto> Download(int id)
        {
            DocumentoResponseDto obj = new DocumentoResponseDto();


            var dato = await _context.DocumentosImpedimento.Where(x => x.Id == id).FirstOrDefaultAsync();
            obj.tipoDocumento = dato.TipoDocumento;
            obj.ruta = dato.rutaFisica;
            obj.nombreArchivo = dato.NombreDocumento;
            return obj;
        }

        public async Task<ImportResponseDto<Impedimento>> ProyectoImport(RequestMasivo data, DatosUsuario usuario)
        {
            ImportResponseDto<Impedimento> dto = new ImportResponseDto<Impedimento>();
            var ProblematicaReal = await _repositoryMantenedores.GetAllByAttribute(Constantes.ProblematicaReal);
            
            var proyectosMasivos = await _context.ProyectoMasivoDetalle.FromSqlInterpolated($"EXEC listaMasiva").ToListAsync();
            var Baremos = await _repositoryMantenedores.GetAllByAttribute("Baremo");
            var base64Content = data.base64;
            var bytes = Convert.FromBase64String(base64Content);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            List<Impedimento> lista = new List<Impedimento>();
            List<Impedimento> listaError = new List<Impedimento>();
            List<Impedimento> listaRepetidos = new List<Impedimento>();
            List<Impedimento> listaInsert = new List<Impedimento>();
            List<Impedimento> listaRepetidosInsert = new List<Impedimento>();

            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Impedimento"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                            var codPry = worksheet.Cells[row, 1].Value?.ToString();
                            var ProbleReal = worksheet.Cells[row, 2].Value?.ToString();
                            var LongImpe = worksheet.Cells[row, 3].Value?.ToString();

                            var dCodPry = proyectosMasivos.Where(x => x.CodigoProyecto == codPry ).FirstOrDefault();
                            var dProReal = ProblematicaReal.Where(x => x.Descripcion == ProbleReal).FirstOrDefault();
                            int baremoId = 0;
                            if (dCodPry != null)
                            {
                                //var baremoAdd = Baremos.Where(x => x.Id == dCodPry.BaremoId).FirstOrDefault();
                                //baremoId = baremoAdd.Id;
                            }
                           


                            if (dCodPry == null || dProReal == null /*|| baremoId==0*/ )
                            {
                                var entidadError = new Impedimento
                                {
                                    LongImpedimento = Convert.ToDecimal(LongImpe)

                                };
                                listaError.Add(entidadError);

                            }
                            else
                            {
                                try
                                {
                                    var entidad = new Impedimento
                                    {
                                        ProyectoId = dCodPry.Id,
                                        ProblematicaRealId = dProReal.Id,
                                        LongImpedimento = Convert.ToDecimal(LongImpe),
                                        CausalReemplazoId = null,
                                        Resuelto = false,
                                        PrimerEstrato = 0,
                                        SegundoEstrato = 0,
                                        TercerEstrato = 0,
                                        CuartoEstrato = 0,
                                        QuintoEstrato = 0,
                                        LongitudReemplazo = 0,
                                        ValidacionCargoPlano = false,
                                        ValidacionCargoSustentoRRCC = false,
                                        ValidacionCargoSustentoAmbiental = false,
                                        ValidacionCargoSustentoArqueologia = false,
                                        ValidacionLegalId = null,
                                        Comentario = "",
                                        FechaPresentacion = null,
                                        FechaPresentacionReemplazo = null,
                                        fechamodifica = DateTime.Now,
                                        FechaRegistro = DateTime.Now,
                                        UsuarioRegisterId = null,
                                        UsuarioModificaId = null,
                                        CostoInversion = 0,
                                        estado = true,
                                        Reemplazado = false

                                    };
                                    lista.Add(entidad);
                                }
                                catch (Exception e)
                                {
                                    var entidadError = new Impedimento
                                    {
                                        LongImpedimento = Convert.ToDecimal(LongImpe),

                                    };
                                    listaError.Add(entidadError);

                                }

                            }


                        }
                        foreach (var item in lista)
                        {
                            listaInsert.Add(item);
                        }
                       
                        if (listaInsert.Count > 0)
                        {
                            await _context.BulkInsertAsync(listaInsert);
                            await _context.SaveChangesAsync();

                        }
                       


                    }
                }


                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , Importar").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Impedimento";
                    trazabilidad.Evento = "Importar";
                    trazabilidad.DescripcionEvento = $"Se crearon masivamente {listaInsert.Count()} impeimentos ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }
                dto.listaError = listaError;
                dto.listaRepetidos = null;
                dto.listaInsert = listaInsert;
                dto.Satisfactorios = listaInsert.Count();
                dto.Error = listaError.Count();
                dto.Actualizados = 0;
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

        public async Task<ResponseDTO> Delete(int id, DatosUsuario usuario)
        {
            try
            {
                var existe = await _context.Impedimento.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (existe!=null)
                {
                    existe.estado = false;
                    _context.Update(existe);
                    await _context.SaveChangesAsync();

                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , Eliminar").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Impedimento";
                        trazabilidad.Evento = "Eliminar";
                        trazabilidad.DescripcionEvento = $"Se eliminó correctamente el impedimento {existe.Id}  ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                    var resultado = new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.EliminacionSatisfactoria
                    };
                    return resultado;
                }
                else
                {
                    var resultado = new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.BusquedaNoExitosa
                    };
                    return resultado;
                }
            }
            catch (Exception e)
            {

                var resultado = new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
                return resultado;
            }
        }
        public async Task<ResponseDTO> DeleteDocumentos(int id, DatosUsuario usuario)
        {
            try
            {
                var existe = await _context.DocumentosImpedimento.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (existe != null)
                {
                    existe.Estado = false;
                    _context.Update(existe);
                    await _context.SaveChangesAsync();


                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO Impedimento , EliminarDocumento").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "Impedimento";
                        trazabilidad.Evento = "EliminarDocumento";
                        trazabilidad.DescripcionEvento = $"Se eliminó correctamente el documento {existe.CodigoDocumento} de la zona de impeimentos ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }
                    var resultado = new ResponseDTO
                    {
                        Valid = true,
                        Message = Constantes.EliminacionSatisfactoria
                    };

                    return resultado;
                }
                else
                {
                    var resultado = new ResponseDTO
                    {
                        Valid = false,
                        Message = Constantes.BusquedaNoExitosa
                    };
                    return resultado;
                }
            }
            catch (Exception e)
            {

                var resultado = new ResponseDTO
                {
                    Valid = false,
                    Message = Constantes.ErrorSistema
                };
                return resultado;
            }
        }
        public static List<CorreoTabla> CompararPropiedades(object valOriginal, object valModificado, string cod_mod, string nomCompleto)
        {
            List<CorreoTabla> camposModificados = new List<CorreoTabla>();
            DateTime fechaActual = DateTime.Today;
            string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
            Type tipo = valOriginal.GetType();

            // Obtener las propiedades del tipo
            PropertyInfo[] propiedades = tipo.GetProperties();
            // Comparar las propiedades 
            foreach (PropertyInfo propiedad in propiedades)
            {
                object valor1 = propiedad.GetValue(valOriginal);
                object valor2 = propiedad.GetValue(valModificado);
                string desCampo = "";
                var descriptionAttribute = propiedad.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    desCampo = descriptionAttribute.Description;
                }

                if (!valor1.Equals(valor2))
                {
                    CorreoTabla fila = new CorreoTabla
                    {
                        codigo = cod_mod,
                        campoModificado = desCampo,
                        valorModificado = propiedad.GetValue(valModificado).ToString(),
                        fechaMod = fechaFormateada,
                        usuModif = nomCompleto,

                    };
                    camposModificados.Add(fila);
                }
            }

            return camposModificados;
        }
    }
}
