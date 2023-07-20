using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;

namespace PlanQuinquenal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IPlanQuinquenalesRepository _planQuinquenalesRepository;
        private readonly IProyectoRepository _proyectoRepository;

        public ReportesController(IPlanQuinquenalesRepository planQuinquenalesRepository, IProyectoRepository proyectoRepository)
        {
            this._planQuinquenalesRepository = planQuinquenalesRepository;
            this._proyectoRepository = proyectoRepository;
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
                for (int i = 1; i <= 7; i++)
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

                if(resultado != null)
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
                for (int i = 1; i <= 7; i++)
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
            p.RecordsPorPagina = 999999;
            var resultado = await _proyectoRepository.GetAll2(p);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proyectos");
                var currentRow = 1;
                for (int i = 1; i <= 24; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }
                    worksheet.Cell(currentRow, 1).Value = "CodigoProyecto";
                    worksheet.Cell(currentRow, 2).Value = "Denominación";
                    worksheet.Cell(currentRow, 3).Value = "Plan Quinquenal";
                    worksheet.Cell(currentRow, 4).Value = "Plan Anual";
                    worksheet.Cell(currentRow, 5).Value = "Etapa";
                    worksheet.Cell(currentRow, 6).Value = "CodigoMalla";
                    worksheet.Cell(currentRow, 7).Value = "Material";
                    worksheet.Cell(currentRow, 8).Value = "Distrito";
                    worksheet.Cell(currentRow, 9).Value = "Constructor";
                    worksheet.Cell(currentRow, 10).Value = "TipoProyecto";
                    worksheet.Cell(currentRow, 11).Value = "TipoRegistro";
                    worksheet.Cell(currentRow, 12).Value = "IngenieroResponsable";
                    worksheet.Cell(currentRow, 13).Value = "Longitud Aprobadda PA";
                    worksheet.Cell(currentRow, 14).Value = "Longitud Real PEndiente";
                    worksheet.Cell(currentRow, 15).Value = "Longitud de Impedimentos";
                    worksheet.Cell(currentRow, 16).Value = "Longitud Real Habilitada";
                    worksheet.Cell(currentRow, 17).Value = "Longitud Reemplazada";
                    worksheet.Cell(currentRow, 18).Value = "Longitud Pendiente Ejecución";
                    worksheet.Cell(currentRow, 19).Value = "LongProyectos";
                    worksheet.Cell(currentRow, 20).Value = "FechaGasificacion";
                    worksheet.Cell(currentRow, 21).Value = "EstadoGeneral";
                    worksheet.Cell(currentRow, 22).Value = "Porcentaje de Avance";
                    worksheet.Cell(currentRow, 23).Value = "Fecha de Registro";
                    worksheet.Cell(currentRow, 24).Value = "Fecha de Modificación";

                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.CodigoProyecto;
                        worksheet.Cell(currentRow, 2).Value = item.descripcion;
                        worksheet.Cell(currentRow, 3).Value = item.AnioPlanPQ;
                        worksheet.Cell(currentRow, 4).Value = item.AnioPlanPA;
                        worksheet.Cell(currentRow, 5).Value = item.Etapa;
                        worksheet.Cell(currentRow, 6).Value = item.CodigoMalla;
                        worksheet.Cell(currentRow, 7).Value = item.Material;
                        worksheet.Cell(currentRow, 8).Value = item.Distrito;
                        worksheet.Cell(currentRow, 9).Value = item.Constructor;
                        worksheet.Cell(currentRow, 10).Value = item.TipoProyecto;
                        worksheet.Cell(currentRow, 11).Value = item.TipoRegistro;
                        worksheet.Cell(currentRow, 12).Value = item.IngenieroResponsable;
                        worksheet.Cell(currentRow, 13).Value = item.LongAprobPa;
                        worksheet.Cell(currentRow, 14).Value = item.LongRealPend;
                        worksheet.Cell(currentRow, 15).Value = item.LongImpedimentos;
                        worksheet.Cell(currentRow, 16).Value = item.LongRealHab;
                        worksheet.Cell(currentRow, 17).Value = item.LongReemplazada;
                        worksheet.Cell(currentRow, 18).Value = item.longPendienteEjecucion;
                        worksheet.Cell(currentRow, 19).Value = item.LongProyectos;
                        worksheet.Cell(currentRow, 20).Value = item.FechaGasificacion;
                        worksheet.Cell(currentRow, 21).Value = item.EstadoGeneral;
                        worksheet.Cell(currentRow, 22).Value = item.PorcentajeAvance;
                        worksheet.Cell(currentRow, 23).Value = item.FechaRegistro;
                        worksheet.Cell(currentRow, 24).Value = item.fechamodifica;

                        worksheet.Cell(currentRow, 4).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
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
        [HttpPost("ListarProyectosSeleccionados")]
        public async Task<IActionResult> ListarProyectosSeleccionados(PlanQuinquenalSelectedId p)
        {
            
            var resultado = await _proyectoRepository.GetSeleccionados(p);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proyectos");
                var currentRow = 1;
                for (int i = 1; i <= 24; i++)
                {
                    //worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#2ec6ff");
                    //worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    //worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, i).Style.Font.SetBold();
                    worksheet.Cell(currentRow, i).Style.Font.FontColor = XLColor.Black;
                }
                worksheet.Cell(currentRow, 1).Value = "CodigoProyecto";
                worksheet.Cell(currentRow, 2).Value = "Denominación";
                worksheet.Cell(currentRow, 3).Value = "Plan Quinquenal";
                worksheet.Cell(currentRow, 4).Value = "Plan Anual";
                worksheet.Cell(currentRow, 5).Value = "Etapa";
                worksheet.Cell(currentRow, 6).Value = "CodigoMalla";
                worksheet.Cell(currentRow, 7).Value = "Material";
                worksheet.Cell(currentRow, 8).Value = "Distrito";
                worksheet.Cell(currentRow, 9).Value = "Constructor";
                worksheet.Cell(currentRow, 10).Value = "TipoProyecto";
                worksheet.Cell(currentRow, 11).Value = "TipoRegistro";
                worksheet.Cell(currentRow, 12).Value = "IngenieroResponsable";
                worksheet.Cell(currentRow, 13).Value = "Longitud Aprobadda PA";
                worksheet.Cell(currentRow, 14).Value = "Longitud Real PEndiente";
                worksheet.Cell(currentRow, 15).Value = "Longitud de Impedimentos";
                worksheet.Cell(currentRow, 16).Value = "Longitud Real Habilitada";
                worksheet.Cell(currentRow, 17).Value = "Longitud Reemplazada";
                worksheet.Cell(currentRow, 18).Value = "Longitud Pendiente Ejecución";
                worksheet.Cell(currentRow, 19).Value = "LongProyectos";
                worksheet.Cell(currentRow, 20).Value = "FechaGasificacion";
                worksheet.Cell(currentRow, 21).Value = "EstadoGeneral";
                worksheet.Cell(currentRow, 22).Value = "Porcentaje de Avance";
                worksheet.Cell(currentRow, 23).Value = "Fecha de Registro";
                worksheet.Cell(currentRow, 24).Value = "Fecha de Modificación";

                if (resultado != null)
                {
                    foreach (var item in resultado.Model)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.CodigoProyecto;
                        worksheet.Cell(currentRow, 2).Value = item.descripcion;
                        worksheet.Cell(currentRow, 3).Value = item.AnioPlanPQ;
                        worksheet.Cell(currentRow, 4).Value = item.AnioPlanPA;
                        worksheet.Cell(currentRow, 5).Value = item.Etapa;
                        worksheet.Cell(currentRow, 6).Value = item.CodigoMalla;
                        worksheet.Cell(currentRow, 7).Value = item.Material;
                        worksheet.Cell(currentRow, 8).Value = item.Distrito;
                        worksheet.Cell(currentRow, 9).Value = item.Constructor;
                        worksheet.Cell(currentRow, 10).Value = item.TipoProyecto;
                        worksheet.Cell(currentRow, 11).Value = item.TipoRegistro;
                        worksheet.Cell(currentRow, 12).Value = item.IngenieroResponsable;
                        worksheet.Cell(currentRow, 13).Value = item.LongAprobPa;
                        worksheet.Cell(currentRow, 14).Value = item.LongRealPend;
                        worksheet.Cell(currentRow, 15).Value = item.LongImpedimentos;
                        worksheet.Cell(currentRow, 16).Value = item.LongRealHab;
                        worksheet.Cell(currentRow, 17).Value = item.LongReemplazada;
                        worksheet.Cell(currentRow, 18).Value = item.longPendienteEjecucion;
                        worksheet.Cell(currentRow, 19).Value = item.LongProyectos;
                        worksheet.Cell(currentRow, 20).Value = item.FechaGasificacion;
                        worksheet.Cell(currentRow, 21).Value = item.EstadoGeneral;
                        worksheet.Cell(currentRow, 22).Value = item.PorcentajeAvance;
                        worksheet.Cell(currentRow, 23).Value = item.FechaRegistro;
                        worksheet.Cell(currentRow, 24).Value = item.fechamodifica;

                        worksheet.Cell(currentRow, 4).Style.DateFormat.Format = "dd/MM/yyyy hh:mm";
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
