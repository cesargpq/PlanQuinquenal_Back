using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.events.IndexEvents;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IRepositoryMantenedores _repositoryMantenedores;

        public DashboardRepository(PlanQuinquenalContext context, IRepositoryMantenedores repositoryMantenedores)
        {
            this._context = context;
            this._repositoryMantenedores = repositoryMantenedores;
        }

        public async Task<MensualidadDTO> ListarAvanceMensual(AvanceMensualDto o)
        {
            
                if (o.AnioPQ.Equals(""))
                {
                    o.AnioPQ = "NO";
                }

                string anioInicial = "";
                string anioFin = "";
                if (o.TipoProy == 0)
                {
                    var qnq = await _context.PQuinquenal.Where(x => x.Id == o.CodigoPlan).FirstOrDefaultAsync();
                     anioInicial = qnq.AnioPlan.ToString().Split("-")[0];
                     anioFin = qnq.AnioPlan.ToString().Split("-")[1];
                }
                else
                {
                    var anual = await _context.PlanAnual.Where(x => x.Id == o.CodigoPlan).FirstOrDefaultAsync();
                    anioInicial = anual.AnioPlan.ToString();
                    anioFin = anual.AnioPlan.ToString();
                }
                
               
                List<string> aniosqnq = new List<string>();
                for (int i = Convert.ToInt32(anioInicial); i <= Convert.ToInt32(anioFin); i++)
                {
                    aniosqnq.Add(i.ToString());
                }
                var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIO  {o.TipoProy}, {o.CodigoPlan}, {o.AnioPQ}, {o.MaterialId}").ToListAsync();

                var listaMeses = resultad.Select(x => x.FechaGasificacion).Distinct().ToList();
                var agrupado = resultad.GroupBy(x => x.FechaGasificacion);

                var grupos = resultad.GroupBy(d => d.FechaGasificacion)
                               .Select(grupo => new
                               {
                                   FechaGasificacion = grupo.Key,
                                   AniosPQ = grupo.Select(d => d.CodigoPlan).Distinct().OrderBy(a => a).ToList(),
                                   Valores = grupo.Select(d => d.LongitudConstruida).ToList()
                               });

                // Crear el objeto en el formato deseado
                var series = new List<object>();
                foreach (var grupo in grupos)
                {
                    var data = new List<double>();
                    for (int i = Convert.ToInt32(anioInicial); i <= Convert.ToInt32(anioFin); i++)
                    {
                        if (grupo.AniosPQ.Contains(i.ToString()))
                        {
                            data.Add(Convert.ToDouble(grupo.Valores[grupo.AniosPQ.IndexOf(i.ToString())]));
                        }
                        else
                        {
                            data.Add(0);
                        }
                    }

                    var serie = new
                    {
                        type = "column",
                        name = grupo.FechaGasificacion.ToString(),
                        data = data
                    };

                    series.Add(serie);
                }

                List<ListaPqMensual> listaMensual = new List<ListaPqMensual>();

                MensualidadDTO mensual = new MensualidadDTO();
                mensual.categorias = listaMeses;
                mensual.ListaPqMensual = series;
                return mensual;
           
            
           
        }




        public async Task<MensualidadDTO> ListarAvanceMensualConstruida(AvanceMensualDto o)
        {

            if (o.AnioPQ.Equals(""))
            {
                o.AnioPQ = "NO";
            }

            string anioInicial = "";
            string anioFin = "";
            if (o.TipoProy == 0)
            {
                var qnq = await _context.PQuinquenal.Where(x => x.Id == o.CodigoPlan).FirstOrDefaultAsync();
                anioInicial = qnq.AnioPlan.ToString().Split("-")[0];
                anioFin = qnq.AnioPlan.ToString().Split("-")[1];
            }
            else
            {
                var anual = await _context.PlanAnual.Where(x => x.Id == o.CodigoPlan).FirstOrDefaultAsync();
                anioInicial = anual.AnioPlan.ToString();
                anioFin = anual.AnioPlan.ToString();
            }


            List<string> aniosqnq = new List<string>();
            for (int i = Convert.ToInt32(anioInicial); i <= Convert.ToInt32(anioFin); i++)
            {
                aniosqnq.Add(i.ToString());
            }
            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIOCONSTRUIDA  {o.TipoProy}, {o.CodigoPlan}, {o.AnioPQ}, {o.MaterialId}").ToListAsync();

            var listaMeses = resultad.Select(x => x.FechaGasificacion).Distinct().ToList();
            var agrupado = resultad.GroupBy(x => x.FechaGasificacion);

            var grupos = resultad.GroupBy(d => d.FechaGasificacion)
                           .Select(grupo => new
                           {
                               FechaGasificacion = grupo.Key,
                               AniosPQ = grupo.Select(d => d.CodigoPlan).Distinct().OrderBy(a => a).ToList(),
                               Valores = grupo.Select(d => d.LongitudConstruida).ToList()
                           });

            // Crear el objeto en el formato deseado
            var series = new List<object>();
            foreach (var grupo in grupos)
            {
                var data = new List<double>();
                for (int i = Convert.ToInt32(anioInicial); i <= Convert.ToInt32(anioFin); i++)
                {
                    if (grupo.AniosPQ.Contains(i.ToString()))
                    {
                        data.Add(Convert.ToDouble(grupo.Valores[grupo.AniosPQ.IndexOf(i.ToString())]));
                    }
                    else
                    {
                        data.Add(0);
                    }
                }

                var serie = new
                {
                    type = "column",
                    name = grupo.FechaGasificacion.ToString(),
                    data = data
                };

                series.Add(serie);
            }

            List<ListaPqMensual> listaMensual = new List<ListaPqMensual>();

            MensualidadDTO mensual = new MensualidadDTO();
            mensual.categorias = listaMeses;
            mensual.ListaPqMensual = series;
            return mensual;

        }


        public async Task<ReporteMaterialDetalle> ListarMaterial(RequestDashboradDTO o)
        {
            var resultad = await _context.ReporteMaterialDetalle.FromSqlRaw($"EXEC dashboardMaterial  {o.MaterialId} , {o.CodigoPlan} , {o.tipoProy}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> habilitado = new List<Decimal>();
            resultad = resultad.OrderByDescending(x => x.LongitudAprobada).ToList();
            foreach (var item in resultad)
            {

                categorias.Add( item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudAprobada + " " + "&nbsp;&nbsp;&nbsp;&nbsp;" + " " + item.Planificado+ "%");
                pendiente.Add(item.longitudPendiente);
                habilitado.Add(item.LongitudConstruida);
            }

            var datos = new ReporteMaterialDetalle
            {
                categorias = categorias,
                pendiente = pendiente,
                habilitado = habilitado
            };
            return datos;

        }

        public async Task<ReporteMaterialConstruidaDetalle> ListarMaterialConstruida(RequestDashboradDTO o)
        {
            var resultad = await _context.ReporteMaterialConstruidaDetalle.FromSqlRaw($"EXEC dashboardMaterialConstruida  {o.MaterialId} , {o.CodigoPlan} , {o.tipoProy}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> construido = new List<Decimal>();
            resultad = resultad.OrderByDescending(x => x.LongitudAprobada).ToList();
            foreach (var item in resultad)
            {

                categorias.Add(item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudAprobada + " " + "&nbsp;&nbsp;&nbsp;&nbsp;" + " " + item.Planificado+ "%");
                pendiente.Add(item.longitudPendiente);
                construido.Add(item.LongitudConstruida);
                
            }

            var datos = new ReporteMaterialConstruidaDetalle
            {
                categorias = categorias,
                pendiente = pendiente,
                construido = construido
            };
            return datos;

        }

        public async Task<ResposeDistritosDetalleDTO> ListarPermisos(RequestDashboradDTO o)
        {
            var resultad = await _context.DistritosPermisoDTO.FromSqlRaw($"EXEC TOTAL {o.MaterialId}, {o.CodigoPlan} , {o.tipoProy}").ToListAsync();

            List<string> categorias = new List<string>();
            List<int> norequiere = new List<int>();
            List<int> permisodenegado = new List<int>();
            List<int> permisotramite = new List<int>();
            List<int> permisonotramitado = new List<int>();
            List<int> permisootorgado = new List<int>();
            List<int> sap = new List<int>();
            List<Decimal> habilitado = new List<Decimal>();

            foreach (var item in resultad)
            {
                categorias.Add(item.Descripcion + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.CantidadTotal + " " + "&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            foreach (var item in resultad)
            {
                var norequierecount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 1 , {o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}  ").ToListAsync();
                norequiere.Add(norequierecount.Count);
                var permisodenegadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 2 , {o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}  ").ToListAsync();
                permisodenegado.Add(permisodenegadocount.Count);
                var permisotramitecount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 3 , {o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}  ").ToListAsync();
                permisotramite.Add(permisotramitecount.Count);
                var permisonotramitadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 4 , {o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}   ").ToListAsync();
                permisonotramitado.Add(permisonotramitadocount.Count);
                var permisootorgadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 5 ,{o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}   ").ToListAsync();
                permisootorgado.Add(permisootorgadocount.Count);
                var sapcount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 6 ,{o.MaterialId}, {item.Descripcion} , {o.CodigoPlan}   ").ToListAsync();
                sap.Add(sapcount.Count);
            }
            var datos = new ResposeDistritosDetalleDTO
            {
                categorias = categorias,
                norequiere = norequiere,
                permisodenegado = permisodenegado,
                permisonotramitado = permisonotramitado,
                permisootorgado     = permisootorgado,
                permisotramite = permisotramite,
                sap = sap
            };

            return datos;
        }
    }
}
