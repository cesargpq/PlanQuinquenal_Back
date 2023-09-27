using ClosedXML.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using System.Security.Claims;

namespace PlanQuinquenal.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IPlanQuinquenalesRepository _planQuinquenalesRepository;
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public ReportesController(IPlanQuinquenalesRepository planQuinquenalesRepository, IProyectoRepository proyectoRepository, IRepositoryMantenedores repositoryMantenedores)
        {
            this._planQuinquenalesRepository = planQuinquenalesRepository;
            this._proyectoRepository = proyectoRepository;
            this._repositoryMantenedores = repositoryMantenedores;
        }
        [HttpPost("ListarPlanQuinquenal")]
        public async Task<IActionResult> ListarPlanQuinquenal(PQuinquenalRequestDTO p)
        {
            p.RecordsPorPagina = 999999;
            var resultado =await  _planQuinquenalesRepository.GetAll(p);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Plan Quinquenal");
                var currentRow = 1;
                for (int i = 1; i <= 9; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }
                worksheet.Cell(currentRow, 1).Value = "Plan Quinquenal";
                worksheet.Cell(currentRow, 2).Value = "Estado de Aprobación";
                worksheet.Cell(currentRow, 3).Value = "Fecha de Registro";
                worksheet.Cell(currentRow, 4).Value = "Fecha de modificación";
                worksheet.Cell(currentRow, 5).Value = "Usuario que registró";
                worksheet.Cell(currentRow, 6).Value = "Usuario que modificó";
                worksheet.Cell(currentRow, 7).Value = "Avance";
                worksheet.Cell(currentRow, 8).Value = "Fecha Aprobacion";
                worksheet.Cell(currentRow, 9).Value = "Descripción";

                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.AnioPlan;
                        worksheet.Cell(currentRow, 2).Value = item.EstadoAprobacion;
                        worksheet.Cell(currentRow, 3).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
                        worksheet.Cell(currentRow, 3).Value = item.FechaRegistro;
                        worksheet.Cell(currentRow, 4).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
                        worksheet.Cell(currentRow, 4).Value = item.FechaModifica;
                        worksheet.Cell(currentRow, 5).Value = item.UsuarioRegister;
                        worksheet.Cell(currentRow, 6).Value = item.UsuarioModifica;
                        worksheet.Cell(currentRow, 7).Value = item.Avance;
                        worksheet.Cell(currentRow, 8).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
                        worksheet.Cell(currentRow, 8).Value = item.FechaAprobacion;
                        worksheet.Cell(currentRow, 9).Value = item.Descripcion;
                    }
                    worksheet.Columns().AdjustToContents();
                    
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlanQuinquenal.xlsx");
                }
            }
        }

        [HttpPost("ListarPqSeleccionados")]
        public async Task<IActionResult> ListarPqSeleccionados(PlanQuinquenalSelectedId p)
        {

            var resultado = await _planQuinquenalesRepository.GetSeleccionados(p);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Plan Quinquenal");
                var currentRow = 1;
                for (int i = 1; i <= 9; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }
                worksheet.Cell(currentRow, 1).Value = "Plan Anual";
                worksheet.Cell(currentRow, 2).Value = "Estado de Aprobación";
                worksheet.Cell(currentRow, 3).Value = "Fecha de Registro";
                worksheet.Cell(currentRow, 4).Value = "Fecha de modificación";
                worksheet.Cell(currentRow, 5).Value = "Usuario que registró";
                worksheet.Cell(currentRow, 6).Value = "Usuario que modificó";
                worksheet.Cell(currentRow, 7).Value = "Avance";
                worksheet.Cell(currentRow, 8).Value = "Fecha Aprobacion";
                worksheet.Cell(currentRow, 9).Value = "Descripción";

                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.AnioPlan;
                        worksheet.Cell(currentRow, 2).Value = item.EstadoAprobacion;
                        worksheet.Cell(currentRow, 3).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 3).Value = item.FechaRegistro;
                        worksheet.Cell(currentRow, 4).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 4).Value = item.FechaModifica;
                        worksheet.Cell(currentRow, 5).Value = item.UsuarioRegister;
                        worksheet.Cell(currentRow, 6).Value = item.UsuarioModifica;
                        worksheet.Cell(currentRow, 7).Value = item.Avance;
                        worksheet.Cell(currentRow, 8).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 8).Value = item.FechaAprobacion;
                        worksheet.Cell(currentRow, 9).Value = item.Descripcion;
                    }
                    worksheet.Columns().AdjustToContents();

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlanQuinquenal.xlsx");
                }
            }
        }

        [HttpPost("ListarProyectos")]
        public async Task<IActionResult> ListarProyectos(FiltersProyectos p)
        {
            p.RecordsPorPagina = 999999999;
            var resultado = await _proyectoRepository.GetAll2(p);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proyectos");
                var currentRow = 1;
                for (int i = 1; i <= 27; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }
                    worksheet.Cell(currentRow, 1).Value = "Código de Proyecto";
                    worksheet.Cell(currentRow, 2).Value = "Periodo PQ";
                    worksheet.Cell(currentRow, 3).Value = "Plan Anual";
                    worksheet.Cell(currentRow, 4).Value = "Código de Malla";
                    worksheet.Cell(currentRow, 5).Value = "Material";
                    worksheet.Cell(currentRow, 6).Value = "Distrito";
                    worksheet.Cell(currentRow, 7).Value = "Constructor";
                    worksheet.Cell(currentRow, 8).Value = "Tipo de Proyecto";
                    worksheet.Cell(currentRow, 9).Value = "Tipo de Registro";
                    worksheet.Cell(currentRow, 10).Value = "Ingeniero Responsable";
                    worksheet.Cell(currentRow, 11).Value = "Long. Aprobada (m)";
                    worksheet.Cell(currentRow, 12).Value = "Longitud Real Pendiente";
                    worksheet.Cell(currentRow, 13).Value = "Longitud de Impedimentos";
                    worksheet.Cell(currentRow, 14).Value = "Longitud Real Habilitada";
                    worksheet.Cell(currentRow, 15).Value = "Longitud Reemplazada";
                    worksheet.Cell(currentRow, 16).Value = "Longitud Pendiente Ejecución";
                    worksheet.Cell(currentRow, 17).Value = "Longitud Construida";
                    worksheet.Cell(currentRow, 18).Value = "Longitud de Proyectos";
                    worksheet.Cell(currentRow, 19).Value = "Fecha de Gasificación";
                    worksheet.Cell(currentRow, 20).Value = "Estado General";
                    worksheet.Cell(currentRow, 21).Value = "Porcentaje de Avance";
                    worksheet.Cell(currentRow, 22).Value = "Fecha de Registro";
                    worksheet.Cell(currentRow, 23).Value = "Fecha de Modificación";
                    worksheet.Cell(currentRow, 24).Value = "Año PQ";
                    worksheet.Cell(currentRow, 25).Value = "Problemática Real";
                    worksheet.Cell(currentRow, 26).Value = "Inversión Aprobada (USD)";
                    worksheet.Cell(currentRow, 27).Value = "Inversión Ejecutada (USD)";
                    worksheet.Cell(currentRow, 28).Value = "Permiso Local";
                    worksheet.Cell(currentRow, 29).Value = "Longitud Local";
                    worksheet.Cell(currentRow, 30).Value = "Permiso Metropolitano";
                    worksheet.Cell(currentRow, 31).Value = "Longitud Metropolitano";
                    worksheet.Cell(currentRow, 32).Value = "Permiso Ambiental";
                    worksheet.Cell(currentRow, 33).Value = "Longitud Ambiental";
                    worksheet.Cell(currentRow, 34).Value = "Permiso Arqueología";
                    worksheet.Cell(currentRow, 35).Value = "Longitud Arqueología";
                    worksheet.Cell(currentRow, 36).Value = "Permisos Social";
                    worksheet.Cell(currentRow, 37).Value = "Longitud Social";


                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.CodigoProyecto;
                        worksheet.Cell(currentRow, 2).Value = item.AnioPlanPQ;
                        worksheet.Cell(currentRow, 3).Value = item.AnioPlanPA;
                        worksheet.Cell(currentRow, 4).Value = item.CodigoMalla;
                        worksheet.Cell(currentRow, 5).Value = item.Material;
                        worksheet.Cell(currentRow, 6).Value = item.Distrito;
                        worksheet.Cell(currentRow, 7).Value = item.Constructor;
                        worksheet.Cell(currentRow, 8).Value = item.TipoProyecto;
                        worksheet.Cell(currentRow, 9).Value = item.TipoRegistro;
                        worksheet.Cell(currentRow, 10).Value = item.IngenieroResponsable;
                        worksheet.Cell(currentRow, 11).Value = item.LongAprobPa;
                        worksheet.Cell(currentRow, 12).Value = item.LongRealPend;
                        worksheet.Cell(currentRow, 13).Value = item.LongImpedimentos;
                        worksheet.Cell(currentRow, 14).Value = item.LongRealHab;
                        worksheet.Cell(currentRow, 15).Value = item.LongReemplazada;
                        worksheet.Cell(currentRow, 16).Value = item.longPendienteEjecucion;
                        worksheet.Cell(currentRow, 17).Value = item.LongConstruida;
                        worksheet.Cell(currentRow, 18).Value = item.LongProyectos;
                        worksheet.Cell(currentRow, 19).Value = item.FechaGasificacion;
                        worksheet.Cell(currentRow, 20).Value = item.EstadoGeneral;
                        worksheet.Cell(currentRow, 21).Value = item.PorcentajeAvance;
                        worksheet.Cell(currentRow, 22).Value = item.FechaRegistro;
                        worksheet.Cell(currentRow, 23).Value = item.fechamodifica;
                        worksheet.Cell(currentRow, 24).Value = item.AñosPQ;
                        worksheet.Cell(currentRow, 25).Value = item.ProblematicaReal;
                        worksheet.Cell(currentRow, 26).Value = item.InversionEjecutada;
                        worksheet.Cell(currentRow, 27).Value = item.CostoInversion;
                        worksheet.Cell(currentRow, 28).Value = item.PermisoLocal;
                        worksheet.Cell(currentRow, 29).Value = item.LongitudLocal;
                        worksheet.Cell(currentRow, 30).Value = item.PermisoMetropolitano;
                        worksheet.Cell(currentRow, 31).Value = item.LongitudMetropolitano;
                        worksheet.Cell(currentRow, 32).Value = item.PermisoAmbiental;
                        worksheet.Cell(currentRow, 33).Value = item.LongitudAmbiental;
                        worksheet.Cell(currentRow, 34).Value = item.PermisoArqueologia;
                        worksheet.Cell(currentRow, 35).Value = item.LongitudArqueologia;
                        worksheet.Cell(currentRow, 36).Value = item.PermisoSociales;
                        worksheet.Cell(currentRow, 37).Value = item.LongitudSociales;


                        worksheet.Cell(currentRow, 22).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(currentRow, 23).Style.DateFormat.Format = "dd/MM/yyyy";
                    }
                    worksheet.Columns().AdjustToContents();

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Proyectos.xlsx");
                }
            }
        }

        //{

        //    var resultado = await _proyectoRepository.GetSeleccionados(p);
        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Proyectos");
        //        var currentRow = 1;
        //        for (int i = 1; i <= 27; i++)
        //        {
        //            //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
        //            //worksheet.Cell(currentRow, i).Style.Font.SetBold();
        //            //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
        //            worksheet.Cell(currentRow, i).Style.Font.SetBold();
        //            worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
        //        }
        //        worksheet.Cell(currentRow, 1).Value = "Código de Proyecto";
        //        worksheet.Cell(currentRow, 2).Value = "Periodo PQ";
        //        worksheet.Cell(currentRow, 3).Value = "Plan Anual";
        //        worksheet.Cell(currentRow, 4).Value = "Código de Malla";
        //        worksheet.Cell(currentRow, 5).Value = "Material";
        //        worksheet.Cell(currentRow, 6).Value = "Distrito";
        //        worksheet.Cell(currentRow, 7).Value = "Constructor";
        //        worksheet.Cell(currentRow, 8).Value = "Tipo de Proyecto";
        //        worksheet.Cell(currentRow, 9).Value = "Tipo de Registro";
        //        worksheet.Cell(currentRow, 10).Value = "Ingeniero Responsable";
        //        worksheet.Cell(currentRow, 11).Value = "Long. Aprobada (m)";
        //        worksheet.Cell(currentRow, 12).Value = "Longitud Real Pendiente";
        //        worksheet.Cell(currentRow, 13).Value = "Longitud de Impedimentos";
        //        worksheet.Cell(currentRow, 14).Value = "Longitud Real Habilitada";
        //        worksheet.Cell(currentRow, 15).Value = "Longitud Reemplazada";
        //        worksheet.Cell(currentRow, 16).Value = "Longitud Pendiente Ejecución";
        //        worksheet.Cell(currentRow, 17).Value = "Longitud Construida";
        //        worksheet.Cell(currentRow, 18).Value = "Longitud de Proyectos";
        //        worksheet.Cell(currentRow, 19).Value = "Fecha de Gasificación";
        //        worksheet.Cell(currentRow, 20).Value = "Estado General";
        //        worksheet.Cell(currentRow, 21).Value = "Porcentaje de Avance";
        //        worksheet.Cell(currentRow, 22).Value = "Fecha de Registro";
        //        worksheet.Cell(currentRow, 23).Value = "Fecha de Modificación";
        //        worksheet.Cell(currentRow, 24).Value = "Año PQ";
        //        worksheet.Cell(currentRow, 25).Value = "Problemática Real";
        //        worksheet.Cell(currentRow, 26).Value = "Inversión Aprobada (USD)";
        //        worksheet.Cell(currentRow, 27).Value = "Inversión Ejecutada (USD)";
        //        if (resultado != null)
        //        {
        //            foreach (var item in resultado.Model)
        //            {
        //                currentRow++;
        //                worksheet.Cell(currentRow, 1).Value = item.CodigoProyecto;
        //                worksheet.Cell(currentRow, 2).Value = item.AnioPlanPQ;
        //                worksheet.Cell(currentRow, 3).Value = item.AnioPlanPA;
        //                worksheet.Cell(currentRow, 4).Value = item.CodigoMalla;
        //                worksheet.Cell(currentRow, 5).Value = item.Material;
        //                worksheet.Cell(currentRow, 6).Value = item.Distrito;
        //                worksheet.Cell(currentRow, 7).Value = item.Constructor;
        //                worksheet.Cell(currentRow, 8).Value = item.TipoProyecto;
        //                worksheet.Cell(currentRow, 9).Value = item.TipoRegistro;
        //                worksheet.Cell(currentRow, 10).Value = item.IngenieroResponsable;
        //                worksheet.Cell(currentRow, 11).Value = item.LongAprobPa;
        //                worksheet.Cell(currentRow, 12).Value = item.LongRealPend;
        //                worksheet.Cell(currentRow, 13).Value = item.LongImpedimentos;
        //                worksheet.Cell(currentRow, 14).Value = item.LongRealHab;
        //                worksheet.Cell(currentRow, 15).Value = item.LongReemplazada;
        //                worksheet.Cell(currentRow, 16).Value = item.longPendienteEjecucion;
        //                worksheet.Cell(currentRow, 17).Value = item.LongConstruida;
        //                worksheet.Cell(currentRow, 18).Value = item.LongProyectos;
        //                worksheet.Cell(currentRow, 19).Value = item.FechaGasificacion;
        //                worksheet.Cell(currentRow, 20).Value = item.EstadoGeneral;
        //                worksheet.Cell(currentRow, 21).Value = item.PorcentajeAvance;
        //                worksheet.Cell(currentRow, 22).Value = item.FechaRegistro;
        //                worksheet.Cell(currentRow, 23).Value = item.fechamodifica;
        //                worksheet.Cell(currentRow, 24).Value = item.AñosPQ;
        //                worksheet.Cell(currentRow, 25).Value = item.ProblematicaReal;
        //                worksheet.Cell(currentRow, 26).Value = item.InversionEjecutada;
        //                worksheet.Cell(currentRow, 27).Value = item.CostoInversion;

        //                worksheet.Cell(currentRow, 22).Style.DateFormat.Format = "dd/MM/yyyy";
        //                worksheet.Cell(currentRow, 23).Style.DateFormat.Format = "dd/MM/yyyy";
        //            }
        //            worksheet.Columns().AdjustToContents();

        //        }
        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Proyectos.xlsx");
        //        }
        //    }
        //}

        [HttpPost("ListarProyectosSeleccionados")]
        public async Task<IActionResult> ListarProyectosSeleccionados(FiltersProyectos p)
        {

            p.RecordsPorPagina = 999999999;
            //var columnas = await _proy
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var idUser = 0;
            foreach (var item in identity.Claims)
            {
                if (item.Type.Equals("$I$Us$@I@D"))
                {
                    idUser = Convert.ToInt16(item.Value);
                }
            }
            var columnSelected = await _repositoryMantenedores.GetColumnSelected(idUser);

            var resultado = await _proyectoRepository.GetAll2(p);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proyectos");
                var currentRow = 1;
                for (int i = 1; i <= 37; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }

                int columna = 1;

                
                if(columnSelected.Where(x => x.campo.Equals("codigoProyecto")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Código de Proyecto";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("anioPlanPQ")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Periodo PQ";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("anioPlanPA")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Plan Anual";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("codigoMalla")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Código de Malla";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("material")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Material";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("distrito")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "distrito";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("constructor")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Constructor";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("tipoProyecto")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Tipo de Proyecto";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("tipoRegistro")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Tipo de Registro";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("ingenieroResponsable")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Ingeniero Responsable";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longAprobPa")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Long. Aprobada (m)";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longRealPend")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Real Pendiente";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longImpedimentos")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud de Impedimentos";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longRealHab")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Real Habilitada";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longReemplazada")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Reemplazada";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longPendienteEjecucion")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Pendiente Ejecución";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longConstruida")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Construida";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longProyectos")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud de Proyectos";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("fechaGasificacion")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Fecha de Gasificación";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("estadoGeneral")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Estado General";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("porcentajeAvance")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Porcentaje de Avance";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("fechaRegistro")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Fecha de Registro";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("fechaModifica")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Fecha de Modificación";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("añosPQ")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Año PQ";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("problematicaReal")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Problemática Real";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("inversionEjecutada")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Inversión Aprobada (USD)";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("costoInversion")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Inversión Ejecutada (USD)";
                    columna++;
                }

                //PermisosLocal

                if (columnSelected.Where(x => x.campo.Equals("permisoLocal")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Permiso Local";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longitudLocal")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Local";
                    columna++;
                }

                //
                if (columnSelected.Where(x => x.campo.Equals("permisoMetropolitano")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Permiso Metropolitano";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longitudMetropolitano")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Metropolitano";
                    columna++;
                }
                //
                if (columnSelected.Where(x => x.campo.Equals("permisoAmbiental")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Permiso Ambiental";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longitudAmbiental")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Ambiental";
                    columna++;
                }
                //
                if (columnSelected.Where(x => x.campo.Equals("permisoArqueologia")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Permiso Arqueología";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longitudArqueologia")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Arqueología";
                    columna++;
                }
                //
                if (columnSelected.Where(x => x.campo.Equals("permisoSociales")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Permisos Social";
                    columna++;
                }
                if (columnSelected.Where(x => x.campo.Equals("longitudSociales")).Select(x => x.seleccion).First())
                {
                    worksheet.Cell(currentRow, columna).Value = "Longitud Social";
                    columna++;
                }


                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        int columnaImp = 1;
                        currentRow++;
                        if (columnSelected.Where(x => x.campo.Equals("codigoProyecto")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.CodigoProyecto;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("anioPlanPQ")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.AnioPlanPQ;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("anioPlanPA")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.AnioPlanPA;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("codigoMalla")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.CodigoMalla;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("material")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.Material;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("distrito")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.Distrito;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("constructor")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.Constructor;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("tipoProyecto")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.TipoProyecto;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("tipoRegistro")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.TipoRegistro;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("ingenieroResponsable")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.IngenieroResponsable;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longAprobPa")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongAprobPa;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longRealPend")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongRealPend;
                            columnaImp++;
                        }

                        if (columnSelected.Where(x => x.campo.Equals("longImpedimentos")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongImpedimentos;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longRealHab")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongRealHab;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longReemplazada")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongReemplazada;
                            columnaImp++;
                        }

                        if (columnSelected.Where(x => x.campo.Equals("longPendienteEjecucion")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.longPendienteEjecucion;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longConstruida")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongConstruida;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longProyectos")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongProyectos;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("fechaGasificacion")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.FechaGasificacion;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("estadoGeneral")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.EstadoGeneral;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("porcentajeAvance")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PorcentajeAvance;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("fechaRegistro")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.FechaRegistro;
                            worksheet.Cell(currentRow, columnaImp).Style.DateFormat.Format = "dd/MM/yyyy";
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("fechaModifica")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.fechamodifica;
                            worksheet.Cell(currentRow, columnaImp).Style.DateFormat.Format = "dd/MM/yyyy";
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("añosPQ")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.AñosPQ;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("problematicaReal")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.ProblematicaReal;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("inversionEjecutada")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.InversionEjecutada;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("costoInversion")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.CostoInversion;
                            columnaImp++;
                        }
                        //
                        if (columnSelected.Where(x => x.campo.Equals("permisoLocal")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PermisoLocal;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longitudLocal")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongitudLocal;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("permisoMetropolitano")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PermisoMetropolitano;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longitudMetropolitano")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongitudMetropolitano;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("permisoAmbiental")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PermisoAmbiental;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longitudAmbiental")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongitudAmbiental;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("permisoArqueologia")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PermisoArqueologia;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longitudArqueologia")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongitudArqueologia;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("permisoSociales")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.PermisoSociales;
                            columnaImp++;
                        }
                        if (columnSelected.Where(x => x.campo.Equals("longitudSociales")).Select(x => x.seleccion).First())
                        {
                            worksheet.Cell(currentRow, columnaImp).Value = item.LongitudSociales;
                            columnaImp++;
                        }

                    }
                    worksheet.Columns().AdjustToContents();

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Proyectos.xlsx");
                }
            }
        }

    }
}
