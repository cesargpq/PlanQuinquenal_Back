using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Org.BouncyCastle.Utilities.Collections;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;
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

            var planesAnuales = await _repositoryMantenedores.GetAllByAttribute("PlanAnual");

                var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIO  {o.TipoProy}, {o.CodigoPlan}, {o.AnioPQ}, {o.MaterialId}").ToListAsync();

                var listaMesesGasi = resultad.Select(x => x.FechaGasificacion).Distinct().ToList();
                var listaMesesPQ = resultad.Select(x => x.CodigoPlan).Distinct().ToList();

                //var agrupado = resultad.GroupBy(x => x.CodigoPlan);


                var grupos = resultad
                    .GroupBy(r => r.CodigoPlan)
                    .OrderBy(g => g.Key)
                    .ToList();

                List<Serie> series = new List<Serie>();
            List<double> totalApro = new List<double>();
            foreach (var grupo in grupos)
                {
                    var data = new List<double>();
                    double totalHabilitada = 0;
                    double totalAprobada = 0;
                        // Crear una lista de longitud cero para todas las fechas desde 2018 hasta 2022
                        foreach (var item in listaMesesGasi)
                        {
                            double longitud = grupo
                            .Where(r => r.FechaGasificacion == item)
                            .Select(r => Convert.ToDouble(r.LongitudConstruida))
                            .FirstOrDefault();
                            //double longitudApro = grupo
                            //        .Where(r => r.FechaGasificacion == item)
                            //        .Select(r => Convert.ToDouble(r.LongitudAprobada))
                            //        .FirstOrDefault();
                            //totalAprobada +=  longitudApro;

                            totalHabilitada += longitud;
                            data.Add(longitud);
                        }

                //totalApro.Add(totalAprobada);
                data.Add(totalHabilitada);
                Serie  s= new Serie();
                s.type = "column";
                s.name = grupo.Key.ToString();
                s.data = data;
                series.Add(s);
                }

                listaMesesGasi.Add("Longitud Habilitado");
                listaMesesGasi.Add("Longitud Aprobado");
            var resultadAprob = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIOAPROBADO  {o.TipoProy}, {o.CodigoPlan}, {o.AnioPQ}, {o.MaterialId}").ToListAsync();

            var grupoAprob = resultadAprob
                  .GroupBy(r => r.CodigoPlan)
                  .OrderBy(g => g.Key)
                  .ToList();
            foreach (var grupo in grupoAprob)
            {
                double longitud = grupo
                           .Where(r => r.CodigoPlan == grupo.Key)
                           .Select(r => Convert.ToDouble(r.LongitudConstruida))
                           .Sum();
                totalApro.Add(longitud);
            }
                int j = 0;
            if(series.Count > 0)
            {
                for (int i = 0; i < totalApro.Count(); i++)
                {
                    for (int z = j; z < series.Count; z++)
                    {
                        series[z].data.Add(totalApro[i]);
                        break;
                    }
                    j++;
                }
            }
            else
            {
                var grupo = planesAnuales.Where(x => x.Id == o.CodigoPlan).Select(x => x.Descripcion).FirstOrDefault();
                List<double> dataD = new List<double>();
                dataD.Add(0);
                dataD.Add(totalApro[0]);
                Serie s = new Serie();
                s.type = "column";
                s.name = grupo;
                s.data = dataD;
                
                series.Add(s);
            }
               


                MensualidadDTO mensual = new MensualidadDTO();
                mensual.categorias = listaMesesGasi;
                mensual.ListaPqMensual = series;
                return mensual;

        }




        public async Task<MensualidadDTO> ListarAvanceMensualConstruida(AvanceMensualDto o)
        {

            if (o.AnioPQ.Equals(""))
            {
                o.AnioPQ = "NO";
            }


            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIOCONSTRUIDA  {o.TipoProy}, {o.CodigoPlan}, {o.AnioPQ}, {o.MaterialId}").ToListAsync();

            var listaMesesGasi = resultad.Select(x => x.FechaGasificacion).Distinct().ToList();
            var listaMesesPQ = resultad.Select(x => x.CodigoPlan).Distinct().ToList();

            var agrupado = resultad.GroupBy(x => x.CodigoPlan);


            var grupos = resultad
                .GroupBy(r => r.CodigoPlan)
                .OrderBy(g => g.Key)
                .ToList();

            List<Serie> series = new List<Serie>();
            List<double> totalConst = new List<double>();
            foreach (var grupo in grupos)
            {
                var data = new List<double>();
                double totalConstruido = 0;
                // Crear una lista de longitud cero para todas las fechas desde 2018 hasta 2022
                foreach (var item in listaMesesGasi)
                {
                    double longitud = grupo
                    .Where(r => r.FechaGasificacion == item)
                    .Select(r => Convert.ToDouble(r.LongitudConstruida))
                    .FirstOrDefault();
                    totalConstruido += longitud;
                    data.Add(longitud);
                }
                data.Add(totalConstruido);
             
                Serie s = new Serie();
                s.type = "column";
                s.name = grupo.Key.ToString();
                s.data = data;
                series.Add(s);
            }


            listaMesesGasi.Add("Longitud Construida");
          



            MensualidadDTO mensual = new MensualidadDTO();
            mensual.categorias = listaMesesGasi;
            mensual.ListaPqMensual = series;
            return mensual;

        }


        public async Task<ReporteMaterialDetalle> ListarMaterial(RequestDashboradDTO o)
        {
            if (o.anioPq.Equals(""))
            {
                o.anioPq = "NO";
            }
            var resultad = await _context.ReporteMaterialDetalle.FromSqlRaw($"EXEC dashboardMaterial  {o.MaterialId} , {o.CodigoPlan} , {o.tipoProy} , {o.anioPq}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> habilitado = new List<Decimal>();
            decimal pendienteD = 0;
            decimal longitudConsD = 0;
            decimal planiD = 0;
            decimal longiD = 0;
            resultad = resultad.OrderByDescending(x => x.LongitudAprobada).ToList();
            string stringAAgregar = "&nbsp;";
            int maxD = 0;
            foreach (var item in resultad)
            {
                string plani = item.Planificado.ToString();
                string longi = item.LongitudAprobada.ToString();
                string distrito = item.Distrito.ToString();
                //while(plani.Length<=12)
                //{                   
                //    plani = stringAAgregar + plani;
                //}
                //while (longi.Length <= 12)
                //{
                //    longi = stringAAgregar + longi;
                //}
                //while (distrito.Length<= 12)
                //{
                //    distrito = stringAAgregar + distrito;
                //}
               
                //maxD++;
           
                //if (maxD <= 5)
                //{
                    categorias.Add(distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + longi + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + plani + "%");
                    pendiente.Add(item.longitudPendiente);
                    habilitado.Add(item.LongitudConstruida);
                //}
                //else
                //{
                //    planiD += item.Planificado;
                //    longiD += item.LongitudAprobada;
                //    pendienteD += item.longitudPendiente;
                //    longitudConsD += item.LongitudConstruida;
                //}
               
            }
            //categorias.Add("OTROS" + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + longiD + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + planiD /(maxD-5) +" "+"%");
            //pendiente.Add(pendienteD);
            //habilitado.Add(longitudConsD);
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
            if (o.anioPq.Equals(""))
            {
                o.anioPq = "NO";
            }
            var resultad = await _context.ReporteMaterialConstruidaDetalle.FromSqlRaw($"EXEC dashboardMaterialConstruida  {o.MaterialId} , {o.CodigoPlan} , {o.tipoProy} , {o.anioPq}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal?> pendiente = new List<Decimal?>();
            List<Decimal?> construido = new List<Decimal?>();
            resultad = resultad.OrderByDescending(x => x.LongitudAprobada).ToList();
            foreach (var item in resultad)
            {

                categorias.Add(item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudAprobada + "&nbsp;&nbsp;&nbsp;&nbsp;"  + item.Planificado+ "%");
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
            if (o.anioPq.Equals(""))
            {
                o.anioPq = "NO";
            }
            var resultad = await _context.DistritosPermisoDTO.FromSqlRaw($"EXEC TOTAL {o.MaterialId}, {o.CodigoPlan} , {o.tipoProy} , {o.anioPq}").ToListAsync();

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
