using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using OfficeOpenXml;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class ProyectoRepository : IRepositoryProyecto
    {
        private readonly PlanQuinquenalContext _context;

        public ProyectoRepository(PlanQuinquenalContext context)
        {
            _context = context;
        }

        public async Task<Object> NuevoProyecto(ProyectoRequest nvoProyecto)
        {
            try
            {
                var contadorProyecto = await _context.Proyectos
                                     .Where(x => x.cod_pry == nvoProyecto.proyecto.cod_pry)
                                     .Where(x => x.cod_etapa == nvoProyecto.proyecto.cod_etapa)
                                     .AsQueryable().CountAsync();
                if (contadorProyecto > 0)
                {
                    var resp = new
                    {
                        idMensaje = "0",
                        mensaje = "El proyecto con dicha etapa ingresado ya existe "
                    };

                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }
                else
                {
                    string fechaHoraActual = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var nvoProyect = new Proyectos
                    {
                        cod_pry = nvoProyecto.proyecto.cod_pry,
                        cod_PQ = nvoProyecto.proyecto.cod_PQ,
                        anioPQ = nvoProyecto.proyecto.anioPQ,
                        cod_anioPA = nvoProyecto.proyecto.cod_anioPA,
                        cod_material = nvoProyecto.proyecto.cod_material,
                        constructor = nvoProyecto.proyecto.constructor,
                        tipo_reg = nvoProyecto.proyecto.tipo_reg,
                        cod_dist = nvoProyecto.proyecto.cod_dist,
                        long_aprob = nvoProyecto.proyecto.long_aprob,
                        tipo_pry = nvoProyecto.proyecto.tipo_pry,
                        cod_etapa = nvoProyecto.proyecto.cod_etapa,
                        cod_vnr = nvoProyecto.proyecto.cod_vnr
                    };

                    _context.Proyectos.Add(nvoProyect);
                    _context.SaveChanges();

                    var lstUsuarioCreado = await _context.Proyectos
                                     .Where(x => x.cod_pry == nvoProyecto.proyecto.cod_pry)
                                     .Where(x => x.cod_etapa == nvoProyecto.proyecto.cod_etapa)
                                     .AsQueryable().ToListAsync();
                    int codPry = lstUsuarioCreado[0].id;

                    #region Creacion de lista de usuarios interesados
                    foreach (var usuInt in nvoProyecto.lstUsuaInter_Inicial)
                    {
                        var entidad = new UsuariosIntersados_pry
                        {
                            id_pry = codPry,
                            cod_usu = usuInt.cod_usu
                        };
                        _context.UsuariosIntersados_pry.Add(entidad);
                    }
                    #endregion

                    #region Creacion de permisos
                    List<string> lstNombrePermisos = new List<string>();
                    foreach (var listaPermis in nvoProyecto.lstPermisos_Inicial)
                    {
                        string NombreArch = listaPermis.nom_doc;
                        string nombreFinal = NombreArch.Replace(".", $"_{fechaHoraActual}.");
                        var entidad = new Permisos_proyec
                        {
                            id_pry = codPry,
                            cod_tipoPerm = listaPermis.cod_tipoPerm, //1 - proyecto , 2 - PQ 
                            nom_doc = nombreFinal,
                            num_exp = listaPermis.num_exp,
                            fecha_reg = DateTime.Now,
                            mime_type = listaPermis.mime_type
                        };
                        _context.Permisos_proyec.Add(entidad);
                        lstNombrePermisos.Add(nombreFinal);
                    }
                    #endregion

                    #region Creacion de informes y actas
                    foreach (var listaInfActas in nvoProyecto.lstInformes_Inicial)
                    {
                        var entidad = new Informes_actas
                        {
                            id_pry = codPry,
                            cod_tipoSeg = 1, //1 - proyecto , 2 - PQ 
                            cod_tipoDoc = listaInfActas.cod_tipoDoc,//1 - Informe , 2 - Acta 
                            aprobacion = listaInfActas.aprobacion,
                            fecha_emis = DateTime.Now
                        };
                        _context.Informes_actas.Add(entidad);
                    }
                    #endregion

                    #region Creacion de Documentos
                    List<string> lstNombreDocumentos = new List<string>();
                    foreach (var listaDocumentos in nvoProyecto.lstDocumentos_Inicial)
                    {
                        string NombreArch = listaDocumentos.nom_doc;
                        string nombreFinal = NombreArch.Replace(".", $"_{fechaHoraActual}.");
                        var entidad = new Docum_proyecto
                        {
                            id_pry = codPry,
                            nom_doc = nombreFinal,
                            fecha_reg = DateTime.Now,
                            mime_type = listaDocumentos.mime_type
                        };
                        _context.Docum_proyecto.Add(entidad);
                        lstNombreDocumentos.Add(nombreFinal);
                    }
                    #endregion

                    #region Creacion de Comentarios
                    foreach (var listaComentarios in nvoProyecto.lstComentarios_Inicial)
                    {
                        var entidad = new Comentarios_proyec
                        {
                            id_pry = codPry,
                            comentario = listaComentarios.comentario,
                            tipo_coment = listaComentarios.tipo_coment,
                            usuario = listaComentarios.usuario,
                            area= listaComentarios.area,
                            fecha_coment = DateTime.Now
                        };
                        _context.Comentarios_proyec.Add(entidad);
                    }
                    #endregion

                    _context.SaveChanges();

                    var resp = new
                    {
                        idMensaje = "1",
                        mensaje = "Se creó el proyecto correctamente",
                        listaNomDocum = lstNombreDocumentos
                    };

                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }
                
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el proyecto"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }

        public async Task<object> NuevosProyectosMasivo(ProyectoRequest reqMasivo)
        {
            var base64Content = reqMasivo.base64; 
            var bytes = Convert.FromBase64String(base64Content);
            bool flagValid = true;
            var mensajeErrado  = "Los siguientes codigos hubieron errores al crearlos: ";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets["Proyectos"];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            bool flareg = true;
                            var codPry = worksheet.Cells[row, 1].Value?.ToString();
                            try { 
                                if (worksheet.Cells[row, 1].Value?.ToString() == null) { break; }
                                string desPQ = worksheet.Cells[row, 2].Value?.ToString();
                                string desAnioPQ = worksheet.Cells[row, 3].Value?.ToString();
                                string desAnioPA = worksheet.Cells[row, 4].Value?.ToString();
                                string desEtapa = worksheet.Cells[row, 5].Value?.ToString();
                                string codMater = worksheet.Cells[row, 6].Value?.ToString();
                                string codConst = worksheet.Cells[row, 8].Value?.ToString();
                                string tipoReg = worksheet.Cells[row, 10].Value?.ToString();
                                string codDist = worksheet.Cells[row, 11].Value?.ToString();
                                string longAprob = worksheet.Cells[row, 13].Value?.ToString();
                                string codTipoPry = worksheet.Cells[row, 14].Value?.ToString();
                                string codMalla = worksheet.Cells[row, 16].Value?.ToString();
                                string codVNR = worksheet.Cells[row, 17].Value?.ToString();

                                #region
                                //-------------------hacer una consulta para validar la descripcion del PQ----------------------
                                int codPQ = 1;
                                #endregion

                                #region
                                //---------------------hacer una consulta para validar la el año PA-------------------
                                int codPA = 1;
                                #endregion

                                #region
                                //--------------------Grabar registros------------------------
                                if (flareg)
                                {
                                    var entidad = new Proyectos
                                    {
                                        cod_pry = codPry,
                                        cod_PQ = codPQ,
                                        anioPQ = desAnioPQ,
                                        cod_anioPA = codPA,
                                        cod_etapa = int.Parse(desEtapa),
                                        cod_material = int.Parse(codMater),
                                        constructor = codConst,
                                        tipo_reg = tipoReg,
                                        cod_dist = int.Parse(codDist),
                                        long_aprob = int.Parse(longAprob),
                                        tipo_pry = int.Parse(codTipoPry),
                                        cod_malla = codMalla,
                                        cod_vnr = codVNR
                                    };
                                    _context.Proyectos.Add(entidad);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    flagValid = false;
                                    mensajeErrado = mensajeErrado + codPry + ", ";
                                }
                                #endregion
                                

                                
                            }catch(Exception ex)
                            {
                                flagValid = false;
                                mensajeErrado = mensajeErrado + codPry + ", ";
                            }
                        }

                        
                    }
                }

                if (!flagValid)
                {
                    mensajeErrado = mensajeErrado.Remove(mensajeErrado.Length - 1);
                    var resp = new
                    {
                        idMensaje = "1",
                        mensaje = mensajeErrado
                    };
                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }
                else
                {
                    var resp = new
                    {
                        idMensaje = "1",
                        mensaje = "Se crearon los proyectos correctamente"
                    };
                    var json = JsonConvert.SerializeObject(resp);
                    return json;
                }
                

                
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear los proyectos"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

            
        }

        public async Task<List<Proyectos>> ObtenerProyectos(FiltersProyectos filterProyectos)
        {
            List<Proyectos> lstPro = new List<Proyectos>();
            var queryable = _context.Proyectos
                                     .Where(x => filterProyectos.des_pry != "" ? x.des_pry == filterProyectos.des_pry : true)
                                     .Where(x => filterProyectos.cod_pry != "" ? x.cod_pry == filterProyectos.cod_pry : true)
                                     //.Where(x => filterProyectos.nro_exp != null ? x.nro_exp == filterProyectos.nro_exp : true)
                                     //.Where(x => filterProyectos.estado_pry != "" ? x.estado_pry == filterProyectos.estado_pry : true)
                                     .Where(x => filterProyectos.cod_etapa != null ? x.cod_etapa == filterProyectos.cod_etapa : true)
                                     //.Where(x => filterProyectos.por_avance != null ? x.por_avance == filterProyectos.por_avance : true)
                                     .Where(x => filterProyectos.cod_material != null ? x.cod_material == filterProyectos.cod_material : true)
                                     .Where(x => filterProyectos.cod_dist != null ? x.cod_dist == filterProyectos.cod_dist : true)
                                     .Where(x => filterProyectos.tipo_pry != null ? x.tipo_pry == filterProyectos.tipo_pry : true)
                                     .Where(x => filterProyectos.cod_PQ != null ? x.cod_PQ == filterProyectos.cod_PQ : true)
                                     .Where(x => filterProyectos.anioPQ != null ? x.anioPQ == filterProyectos.anioPQ : true)
                                     .Where(x => filterProyectos.cod_anioPA != null ? x.cod_anioPA == filterProyectos.cod_anioPA : true)
                                     .Where(x => filterProyectos.cod_malla != null ? x.cod_malla == filterProyectos.cod_malla : true)
                                     .Where(x => filterProyectos.constructor != null ? x.constructor == filterProyectos.constructor : true)
                                     .Where(x => filterProyectos.ingRespon != null ? x.ingRespon == filterProyectos.ingRespon : true)
                                     .Where(x => filterProyectos.user_reg != null ? x.user_reg == filterProyectos.user_reg : true)
                                     .Where(x => filterProyectos.fecha_gas != null ? x.fecha_gas == filterProyectos.fecha_gas : true)
                                     .Where(x => filterProyectos.cod_pryReemp != null ? x.cod_pryReemp == filterProyectos.cod_pryReemp : true)
                                     .AsQueryable();

            var entidades = await queryable.ToListAsync();
            lstPro = entidades;
            return lstPro;

        }

        public async Task<Object> ActualizarProyecto(Proyectos proyecto)
        {
            try
            {
                var modPry = _context.Proyectos.FirstOrDefault(p => p.id == proyecto.id);
                modPry.des_pry = proyecto.des_pry;
                modPry.cod_pry = proyecto.cod_pry;
                _context.SaveChanges();

                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se modifico el proyecto correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al actualizar el proyecto"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }

        }
        public async Task<Proyectos> ObtenerProyectoxNro(string nroProy)
        {
            Proyectos lstPro = new Proyectos();
            var queryable = _context.Proyectos
                                     .Where(x =>  x.cod_pry == nroProy)
                                     .AsQueryable();

            double cantidad = await queryable.CountAsync();
            if (cantidad == 1)
            {
                var entidades = await queryable.ToListAsync();
                lstPro = entidades[0];
            }
                
            return lstPro;

        }

        public async Task<Object> CrearComentario(Comentarios_proyec comentario)
        {
            try
            {
                var nvoComentario = new Comentarios_proyec
                {
                    id_pry = comentario.id_pry,
                    comentario = comentario.comentario,
                    tipo_coment = comentario.tipo_coment,
                    usuario = comentario.usuario,
                    area = comentario.area,
                    fecha_coment = comentario.fecha_coment
                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Comentarios_proyec.Add(nvoComentario);
                _context.SaveChanges();
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el comentario correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el comentario"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }

        public async Task<Object> CrearImpedimento(ImpedimentoRequest impedimento)
        {
            try
            {
                var nvoImpedimento = new Impedimentos
                {
                     id_pry = impedimento.id_pry,
                     cod_pry = impedimento.cod_pry,
                     cod_pbReal = impedimento.cod_pbReal,
                     long_imped =  impedimento.long_imped,
                     cod_cauReemp = impedimento.cod_cauReemp,
                     estrato1 = impedimento.estrato1,
                     estrato2 = impedimento.estrato2,
                     estrato3 = impedimento.estrato3,
                     estrato4 = impedimento.estrato4,
                     estrato5 = impedimento.estrato5,
                     long_reemp = impedimento.long_reemp,
                     costo_inv = impedimento.costo_inv,
                     valid_cargo_planos = impedimento.valid_cargo_planos,
                     valid_cargo_susAmb = impedimento.valid_cargo_susAmb,
                     valid_cargo_susArq = impedimento.valid_cargo_susArq,
                     valid_cargo_susRRCC = impedimento.valid_cargo_susRRCC,
                     cod_validLegal = impedimento.cod_validLegal,
                     fecha_prestReemp = impedimento.fecha_prestReemp,
                     coment_eva = impedimento.coment_eva

                };

                // Agregar la entidad al objeto DbSet y guardar los cambios en la base de datos
                _context.Impedimentos.Add(nvoImpedimento);
                _context.SaveChanges();
                var resp = new
                {
                    idMensaje = "1",
                    mensaje = "Se creó el impedimento correctamente"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
            catch (Exception ex)
            {
                var resp = new
                {
                    idMensaje = "0",
                    mensaje = "Hubo un error al crear el impedimento"
                };

                var json = JsonConvert.SerializeObject(resp);
                return json;
            }
        }
    }
}
