using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class BolsaReemplazoRepository : IBolsaReemplazoRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryMantenedores _repositoryMantenedores;
        private readonly ITrazabilidadRepository _trazabilidadRepository;
        private readonly IRepositoryNotificaciones _repositoryNotificaciones;

        public BolsaReemplazoRepository(PlanQuinquenalContext context, IMapper mapper, IRepositoryMantenedores repositoryMantenedores,ITrazabilidadRepository trazabilidadRepository, IRepositoryNotificaciones repositoryNotificaciones)
        {
            this._context = context;
            this.mapper = mapper;
            this._repositoryMantenedores = repositoryMantenedores;
            this._trazabilidadRepository = trazabilidadRepository;
            this._repositoryNotificaciones = repositoryNotificaciones;
        }

        public async Task<ResponseDTO> Update(RequestUpdateBolsaDTO p, int id, DatosUsuario usuario)
        {
            var UsuarioReg = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
            string nomPerfil = UsuarioReg[0].Perfil.nombre_perfil;
            string NomCompleto = UsuarioReg[0].nombre_usu.ToString() + " " + UsuarioReg[0].apellido_usu.ToString();
            var br = await _context.BolsaReemplazo.Where(x => x.Id != id && x.CodigoProyecto.Equals(p.CodigoProyecto)).FirstOrDefaultAsync();
            if (br == null)
            {
                var brUpdate = await _context.BolsaReemplazo.Where(x => x.Id == id).FirstOrDefaultAsync();
                RequestUpdateBolsaDTO oldBR = new RequestUpdateBolsaDTO();
                var mapBR = mapper.Map<RequestUpdateBolsaDTO>(brUpdate);
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
                brUpdate.UsuarioModifica = usuario.UsuaroId;
                brUpdate.FechaModifica = DateTime.Now;
                List<CorreoTabla> camposModificados = CompararPropiedadesAsync(brUpdate.CodigoProyecto,brUpdate, NomCompleto).GetAwaiter().GetResult();
                _context.Update(brUpdate);
                await _context.SaveChangesAsync();

                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO BolsaReemplazo , Editar").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "BolsaReemplazo";
                    trazabilidad.Evento = "Editar";
                    trazabilidad.DescripcionEvento = $"Se actualizó correctamente el proyecto {brUpdate.CodigoProyecto} ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }



                #region notificacion
                List<string> correosList = new List<string>();
                string asunto = $"Se modificó el anteproyecto {brUpdate.CodigoProyecto}";
                var UsuarioInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                foreach (var listaUsuInters in UsuarioInt)
                {
                    int cod_usu = listaUsuInters.cod_usu;
                    var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.modImp == true).ToListAsync();
                    string correo = listaUsuInters.correo_usu;
                    if (lstpermisos.Count() == 1)
                    {
                        Notificaciones notifProyecto = new Notificaciones();
                        notifProyecto.cod_usu = cod_usu;
                        notifProyecto.seccion = "Gestión Reemplazo";
                        notifProyecto.nombreComp_usu = NomCompleto;
                        notifProyecto.cod_reg = p.CodigoProyecto.ToString();
                        notifProyecto.area = nomPerfil;
                        notifProyecto.fechora_not = DateTime.Now;
                        notifProyecto.flag_visto = false;
                        notifProyecto.tipo_accion = "M";
                        notifProyecto.mensaje = $"Se modificó el anteproyecto {brUpdate.CodigoProyecto}";
                        notifProyecto.codigo = brUpdate.Id;
                        notifProyecto.modulo = "I";
                        correosList.Add(correo);

                        var respuestNotif = await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                        dynamic objetoNotif = JsonConvert.DeserializeObject(respuestNotif.ToString());
                        int codigoNotifCreada = int.Parse(objetoNotif.codigoNot.ToString());
                        camposModificados.ForEach(item => {
                                    item.idNotif = codigoNotifCreada;
                                    item.id = null;
                                    });
                        _context.CorreoTabla.AddRange(camposModificados);
                        _context.SaveChanges();
                    }
                }

                await _repositoryNotificaciones.EnvioCorreoNotifList(camposModificados, correosList, "M", "Bolsa Reemplazo",asunto);

                #endregion

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
                    if (item.Metadata.Name.Equals("FechaModifica") || item.Metadata.Name.Equals("UsuarioModifica"))
                    {
                        continue;
                    }
                    else if (item.Metadata.Name.Equals("DistritoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Distrito");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Distrito");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("ConstructorId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Constructor");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Constructor");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("ReemplazoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("Reemplazo");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Reemplazo");
                        camposModificados.Add(fila);
                    }
                    else if (item.Metadata.Name.Equals("PermisoId"))
                    {
                        var enumerable = await _repositoryMantenedores.GetAllByAttribute("ZonaPermiso");
                        CorreoTabla fila = CrearCorreoTabla(codigo, nomCompleto, fechaFormateada, item, enumerable, "Zona con Permiso");
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
        public async Task<ResponseDTO> Add(RequestBolsaDto p, DatosUsuario usuario)
        {
            try
            {
                var existe = await _context.BolsaReemplazo.Where(x => x.CodigoProyecto == p.CodigoProyecto).FirstOrDefaultAsync();

                if(existe== null)
                {
                    var map = mapper.Map<BolsaReemplazo>(p);
                    map.FechaModifica = DateTime.Now;
                    map.FechaRegistro = DateTime.Now;
                    map.UsuarioModifica = usuario.UsuaroId;
                    map.UsuarioRegistra = usuario.UsuaroId;
                    map.ConstructorId = p.ContratistaId;
                    map.Estado = true;
                    map.Estrato1 = 0;
                    map.Estrato2 = 0;
                    map.Estrato3 = 0;
                    map.Estrato4 = 0;
                    map.Estrato5 = 0;
                    map.CostoInversion = 0;
                    map.LongitudReemplazo = 0;
                    map.Reemplazado = false;
                    _context.Add(map);
                    await _context.SaveChangesAsync();


                    var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO BolsaReemplazo , Crear").ToListAsync();
                    if (resultad.Count > 0)
                    {
                        Trazabilidad trazabilidad = new Trazabilidad();
                        List<Trazabilidad> listaT = new List<Trazabilidad>();
                        trazabilidad.Tabla = "BolsaReemplazo";
                        trazabilidad.Evento = "Crear";
                        trazabilidad.DescripcionEvento = $"Se creó correctamente el anteproyecto {map.CodigoProyecto} ";
                        trazabilidad.UsuarioId = usuario.UsuaroId;
                        trazabilidad.DireccionIp = usuario.Ip;
                        trazabilidad.FechaRegistro = DateTime.Now;

                        listaT.Add(trazabilidad);
                        await _trazabilidadRepository.Add(listaT);
                    }


                    #region Comparacion de estructuras y agregacion de cambios

                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = map.CodigoProyecto.ToString()
                    };

                    composCorreo.Add(correoDatos);
                    #endregion

                    #region Envio de notificacion
                    List<string> correosList = new List<string>();
                    List<Notificaciones> notificacionList = new List<Notificaciones>();
                    string asunto = $"Se creó el anteproyecto {map.CodigoProyecto}";
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x=>x.estado_user == "A").Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    var usuInt = await _context.Usuario.Where(x=>x.estado_user == "A").ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.cod_usu;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regImp == true).ToListAsync();
                        string correo = listaUsuInters.correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Gestión Reemplazo";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = map.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó el anteproyecto {map.CodigoProyecto}";
                            notifProyecto.codigo = map.Id;
                            notifProyecto.modulo = "A";
                            correosList.Add(correo);
                            notificacionList.Add(notifProyecto);
                        }
                    }
                    await _repositoryNotificaciones.CrearNotificacionList(notificacionList);
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Bolsa Reemplazo", asunto);

                    #endregion




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

        public async Task<ImportResponseDto<BolsaReemplazo>> ImportarMasivo(RequestMasivo data, DatosUsuario usuario)
        {
            ImportResponseDto<BolsaReemplazo> dto = new ImportResponseDto<BolsaReemplazo>();
            
            var bolsaMasivos = await _context.BolsaReemplazo.FromSqlInterpolated($"EXEC LISTARMASIVOBOLSA").ToListAsync();
            var Distritos = await _repositoryMantenedores.GetAllByAttribute(Constantes.Distrito);
            var Constructores = await _repositoryMantenedores.GetAllByAttribute(Constantes.Constructor);
            var zonaPermiso = await _repositoryMantenedores.GetAllByAttribute(Constantes.ZonaPermiso);
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
                            var Estrato1 = worksheet.Cells[row, 5].Value?.ToString();
                            var Estrato2 = worksheet.Cells[row, 6].Value?.ToString();
                            var Estrato3 = worksheet.Cells[row, 7].Value?.ToString();
                            var Estrato4 = worksheet.Cells[row, 8].Value?.ToString();
                            var Estrato5 = worksheet.Cells[row, 9].Value?.ToString();
                            var CostoInversion = worksheet.Cells[row, 10].Value?.ToString();
                            var LongReemplazo = worksheet.Cells[row, 11].Value?.ToString();
                            var RiesgoSocial = worksheet.Cells[row, 12].Value?.ToString();
                            var ZonaPermiso = worksheet.Cells[row, 13].Value?.ToString();
                            var proyecto = bolsaMasivos.Where(x => x.CodigoProyecto.Equals(codPry)).FirstOrDefault();
                            var distritodesc = Distritos.Where(x => x.Descripcion.Equals(distrito)).FirstOrDefault();
                            var contratistadesc = Constructores.Where(x => x.Descripcion.Equals(contratista)).FirstOrDefault();
                            var zonaPermisoDesc = zonaPermiso.Where(x=>x.Descripcion.Equals(ZonaPermiso)).FirstOrDefault();


                            if (  proyecto != null)
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
                                        DistritoId = distritodesc == null ? null : distritodesc.Id,
                                        ConstructorId = contratistadesc == null ? null : contratistadesc.Id,
                                        CodigoMalla = codigoMalla,
                                        FechaModifica = DateTime.Now,
                                        FechaRegistro = DateTime.Now,
                                        UsuarioModifica = usuario.UsuaroId,
                                        UsuarioRegistra = usuario.UsuaroId,
                                        Estado = true,
                                        Estrato1 = Convert.ToInt32(Estrato1),
                                        Estrato2 = Convert.ToInt32(Estrato2),
                                        Estrato3 = Convert.ToInt32(Estrato3),
                                        Estrato4 = Convert.ToInt32(Estrato4),
                                        Estrato5 = Convert.ToInt32(Estrato5),
                                        CostoInversion = Convert.ToDecimal(CostoInversion),
                                        LongitudReemplazo = Convert.ToDecimal(LongReemplazo),
                                        Reemplazado = false,
                                        PermisoId = zonaPermisoDesc == null ? null : zonaPermisoDesc.Id

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
                                        Reemplazado = existes.Reemplazado
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
                             _context.AddRange(listaInsert);
                            await _context.SaveChangesAsync();

                        }
                        if (listaRepetidosInsert.Count > 0)
                        {
                            _context.UpdateRange(listaRepetidosInsert);
                            await _context.SaveChangesAsync();
                        }


                    }
                }

                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO BolsaReemplazo , Importar").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "BolsaReemplazo";
                    trazabilidad.Evento = "Crear Masivo";
                    trazabilidad.DescripcionEvento = $"Se insertó correctamente {listaInsert.Count()} anteproyectos";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }
                dto.listaError = null;
                dto.listaRepetidos = null;
                dto.listaInsert = null;
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

        public async Task<ResponseDTO> GestionReemplazo(GestionReemplazoDto p, DatosUsuario usuario)
        {

            string anioActuales = DateTime.Now.Year.ToString();
            var planAnualActual = await _context.PlanAnual.Where(x => x.AnioPlan.Equals(anioActuales)).FirstOrDefaultAsync();

            if(planAnualActual == null)
            {
                return new ResponseDTO
                {
                    Valid = false,
                    Message = $"No existe el Plan Anual {anioActuales } registrado para realizar el reemplazo "
                };
            }

            foreach (var item in p.Impedimentos)
            {
                var impedimentos = await _context.Impedimento.Where(x => x.Id == item).FirstOrDefaultAsync();
                impedimentos.FechaPresentacionReemplazo = p.FechaPresenacionReemplazo;
                impedimentos.NumeroReemplazo = p.NumeroReemplazo;
                impedimentos.Reemplazado = true;
                impedimentos.LongitudReemplazo = impedimentos.LongImpedimento;
                _context.Update(impedimentos);
            
            }
            foreach (var item in p.BolsaReemplazo)
            {
                var maxReemplaxo = await _context.ReemplazoMaxDTO.FromSqlRaw($"EXEC obtenermaxreemplaxo").ToListAsync();
                var bolsa = await _context.BolsaReemplazo.Where(x => x.Id == item).FirstOrDefaultAsync();
                if(maxReemplaxo.ElementAt(0).Numero != null)
                {
                    
                    bolsa.NumeroReemplazo = maxReemplaxo.ElementAt(0).Numero + 1;
                    bolsa.ReemplazoId = p.NumeroReemplazo;
                    bolsa.FechaPresenacionReemplazo = p.FechaPresenacionReemplazo;
                }
                else
                {
                    bolsa.ReemplazoId = p.NumeroReemplazo;
                    bolsa.FechaPresenacionReemplazo = p.FechaPresenacionReemplazo;
                    bolsa.NumeroReemplazo = 1;
                }
                
                bolsa.Reemplazado = true;
               
             
                Proyecto proyecto= new Proyecto();
                proyecto.CodigoProyecto = bolsa.CodigoProyecto;
                proyecto.PQuinquenalId = null;
                proyecto.AñosPQ =null;
                proyecto.PlanAnualId = planAnualActual.Id == null ? null : planAnualActual.Id;
                proyecto.MaterialId = null;
                proyecto.ConstructorId = bolsa.ConstructorId == null ? null : bolsa.ConstructorId;
                proyecto.TipoRegistroId = null;
                proyecto.LongAprobPa = bolsa.LongitudReemplazo;
                proyecto.LongConstruida = 0;
                proyecto.TipoProyectoId = null;
                proyecto.InversionEjecutada = bolsa.CostoInversion;
                proyecto.descripcion = "";
                proyecto.CodigoMalla = bolsa.CodigoMalla;
                proyecto.IngenieroResponsableId = null;
                proyecto.UsuarioRegisterId = usuario.UsuaroId;
                proyecto.UsuarioModificaId = usuario.UsuaroId;
                proyecto.FechaGasificacion = null;
                proyecto.FechaRegistro = DateTime.Now;
                proyecto.fechamodifica = DateTime.Now;
                //proyecto.LongImpedimentos = 0;
                proyecto.LongRealHab = 0;
                proyecto.LongRealPend = 0;
                proyecto.LongProyectos = 0;
                proyecto.DistritoId = bolsa.DistritoId == null ? null : bolsa.DistritoId;
                proyecto.TipoProy = true;
                _context.Add(proyecto);
                _context.Update(bolsa);
                await _context.SaveChangesAsync();

                var documentos = await _context.DocumentosBR.Where(x=>x.BolsaReemplazoId.Equals(bolsa.Id)).ToListAsync();

                List<DocumentosPy> doc = new List<DocumentosPy>();

                
                foreach (var itemdoc in documentos)
                {
                    DocumentosPy docuobjeto = new DocumentosPy();

                    docuobjeto.CodigoProyecto = proyecto.CodigoProyecto;
                    docuobjeto.CodigoDocumento = itemdoc.CodigoDocumento;
                    docuobjeto.NombreDocumento = itemdoc.NombreDocumento;
                    docuobjeto.FechaEmision = itemdoc.FechaEmision;
                    docuobjeto.TipoDocumento = itemdoc.TipoDocumento;
                    docuobjeto.Ruta = itemdoc.ruta;
                    docuobjeto.Estado = itemdoc.Estado;
                    docuobjeto.rutafisica = itemdoc.rutaFisica;
                    docuobjeto.Aprobaciones = itemdoc.Aprobaciones;
                    doc.Add(docuobjeto);
                }
                if(doc.Count > 0) 
                {
                    _context.AddRange(doc);
                    await _context.SaveChangesAsync();
                   
                }

                var resultad = await _context.TrazabilidadVerifica.FromSqlInterpolated($"EXEC VERIFICAEVENTO BolsaReemplazo , Gestionar").ToListAsync();
                if (resultad.Count > 0)
                {
                    Trazabilidad trazabilidad = new Trazabilidad();
                    List<Trazabilidad> listaT = new List<Trazabilidad>();
                    trazabilidad.Tabla = "Proyecto";
                    trazabilidad.Evento = "Gestión de reemplazo";
                    trazabilidad.DescripcionEvento = $"Se realizó la gestión de reemplazo correctamente ";
                    trazabilidad.UsuarioId = usuario.UsuaroId;
                    trazabilidad.DireccionIp = usuario.Ip;
                    trazabilidad.FechaRegistro = DateTime.Now;

                    listaT.Add(trazabilidad);
                    await _trazabilidadRepository.Add(listaT);
                }




                foreach (var datos in p.Impedimentos)
                {
                    #region Comparacion de estructuras y agregacion de cambios

                    
                    #endregion

                    #region Envio de notificacion
                    var Usuario = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == usuario.UsuaroId).ToListAsync();
                    string nomPerfil = Usuario[0].Perfil.nombre_perfil;
                    string NomCompleto = Usuario[0].nombre_usu.ToString() + " " + Usuario[0].apellido_usu.ToString();
                    var imp = await _context.Impedimento.Where(x => x.Id == datos).FirstOrDefaultAsync();

                    var py = await _context.Proyecto.Where(x => x.Id == imp.ProyectoId).FirstOrDefaultAsync();

                    List<CorreoTabla> composCorreo = new List<CorreoTabla>();
                    CorreoTabla correoDatos = new CorreoTabla
                    {
                        codigo = py.CodigoProyecto.ToString()
                    };

                    composCorreo.Add(correoDatos);
                    List<string> correosList = new List<string>();
                    string asunto = $"Se creó la gestión de reemplazo {py.CodigoProyecto}";
                    var usuInt = await _context.UsuariosInteresadosPy.Where(x => x.CodigoProyecto == py.CodigoProyecto).ToListAsync();
                    foreach (var listaUsuInters in usuInt)
                    {
                        int cod_usu = listaUsuInters.UsuarioId;
                        var lstpermisos = await _context.Config_notificaciones.Where(x => x.cod_usu == cod_usu).Where(x => x.regPry == true).ToListAsync();
                        var UsuarioInt = await _context.Usuario.Include(x => x.Perfil).Where(x => x.cod_usu == cod_usu).ToListAsync();
                        string correo = UsuarioInt[0].correo_usu.ToString();
                        if (lstpermisos.Count() == 1)
                        {
                            Notificaciones notifProyecto = new Notificaciones();
                            notifProyecto.cod_usu = cod_usu;
                            notifProyecto.seccion = "Gestión Reemplazo";
                            notifProyecto.nombreComp_usu = NomCompleto;
                            notifProyecto.cod_reg = py.CodigoProyecto;
                            notifProyecto.area = nomPerfil;
                            notifProyecto.fechora_not = DateTime.Now;
                            notifProyecto.flag_visto = false;
                            notifProyecto.tipo_accion = "C";
                            notifProyecto.mensaje = $"Se creó la gestión de reemplazo {py.CodigoProyecto}";
                            notifProyecto.codigo = py.Id;
                            notifProyecto.modulo = "A";
                            correosList.Add(correo);
                            await _repositoryNotificaciones.CrearNotificacion(notifProyecto);
                            
                        }
                    }
                    await _repositoryNotificaciones.EnvioCorreoNotifList(composCorreo, correosList, "C", "Gestión Reemplazo", asunto);
                    #endregion

                }



            }

            return new ResponseDTO
            {
                Valid = true,
                Message = "Gestión de reemplazo existosa"
            };
        }
    }
}
