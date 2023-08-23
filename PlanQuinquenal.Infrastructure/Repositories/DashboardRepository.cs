using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
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
            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIO  {o.Anio} , {o.MaterialId}").ToListAsync();

            var data = resultad.GroupBy(x=>x.pquinquenalId).ToList();

            var countQuinquenal = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");

            
            List<ListaPqMensual> listaMensual = new List<ListaPqMensual>();
            for (int i = 0; i < countQuinquenal.Count(); i++)
            {
                foreach (var item in data)
                {
                    var exist = item.Where(x => x.pquinquenalId == countQuinquenal.ElementAt(i).Id).FirstOrDefault();
                    ListaPqMensual obj = new ListaPqMensual();
                    
                    HabilitadoDto habilitado = new HabilitadoDto();
                    if (exist != null && exist.pquinquenalId == countQuinquenal.ElementAt(i).Id)
                    {

                        var existeE = item.Where(x => x.Mes.Equals("Enero")).FirstOrDefault();
                        if (existeE != null)
                        {

                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeF = item.Where(x => x.Mes.Equals("Febrero")).FirstOrDefault();
                        if (existeF != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeM = item.Where(x => x.Mes.Equals("Marzo")).FirstOrDefault();
                        if (existeM != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeA = item.Where(x => x.Mes.Equals("Abril")).FirstOrDefault();
                        if (existeA != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeMay = item.Where(x => x.Mes.Equals("Mayo")).FirstOrDefault();
                        if (existeMay != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeJuni = item.Where(x => x.Mes.Equals("Junio")).FirstOrDefault();
                        if (existeJuni != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeJuli = item.Where(x => x.Mes.Equals("Julio")).FirstOrDefault();
                        if (existeJuli != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeAg = item.Where(x => x.Mes.Equals("Agosto")).FirstOrDefault();
                        if (existeAg != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeSep = item.Where(x => x.Mes.Equals("Septiembre")).FirstOrDefault();
                        if (existeSep != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeOc = item.Where(x => x.Mes.Equals("Octubre")).FirstOrDefault();
                        if (existeOc != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeNo = item.Where(x => x.Mes.Equals("Noviembre")).FirstOrDefault();
                        if (existeNo != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeDi = item.Where(x => x.Mes.Equals("Diciembre")).FirstOrDefault();
                        if (existeDi != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }

                        listaMensual.Add(obj);
                        i++;
                    }
                }
            }


            MensualidadDTO mensual = new MensualidadDTO();
            mensual.categorias.Add("Enero");
            mensual.categorias.Add("Febrero");
            mensual.categorias.Add("Marzo");
            mensual.categorias.Add("Abril");
            mensual.categorias.Add("Mayo");
            mensual.categorias.Add("Junio");
            mensual.categorias.Add("Julio");
            mensual.categorias.Add("Agosto");
            mensual.categorias.Add("Septiembre");
            mensual.categorias.Add("Octubre");
            mensual.categorias.Add("Noviembre");
            mensual.categorias.Add("Diciembre");
            mensual.ListaPqMensual = listaMensual;
            return mensual;
        }




        public async Task<MensualidadDTO> ListarAvanceMensualConstruida(AvanceMensualDto o)
        {
            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIOCONSTRUIDA  {o.Anio} , {o.MaterialId}").ToListAsync();

            var data = resultad.GroupBy(x => x.pquinquenalId).ToList();

            var countQuinquenal = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");


            List<ListaPqMensual> listaMensual = new List<ListaPqMensual>();
            for (int i = 0; i < countQuinquenal.Count(); i++)
            {
                foreach (var item in data)
                {
                    var exist = item.Where(x => x.pquinquenalId == countQuinquenal.ElementAt(i).Id).FirstOrDefault();
                    ListaPqMensual obj = new ListaPqMensual();

                    HabilitadoDto habilitado = new HabilitadoDto();
                    if (exist != null && exist.pquinquenalId == countQuinquenal.ElementAt(i).Id)
                    {

                        var existeE = item.Where(x => x.Mes.Equals("Enero")).FirstOrDefault();
                        if (existeE != null)
                        {

                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeF = item.Where(x => x.Mes.Equals("Febrero")).FirstOrDefault();
                        if (existeF != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeM = item.Where(x => x.Mes.Equals("Marzo")).FirstOrDefault();
                        if (existeM != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeA = item.Where(x => x.Mes.Equals("Abril")).FirstOrDefault();
                        if (existeA != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeMay = item.Where(x => x.Mes.Equals("Mayo")).FirstOrDefault();
                        if (existeMay != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeJuni = item.Where(x => x.Mes.Equals("Junio")).FirstOrDefault();
                        if (existeJuni != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeJuli = item.Where(x => x.Mes.Equals("Julio")).FirstOrDefault();
                        if (existeJuli != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeAg = item.Where(x => x.Mes.Equals("Agosto")).FirstOrDefault();
                        if (existeAg != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeSep = item.Where(x => x.Mes.Equals("Septiembre")).FirstOrDefault();
                        if (existeSep != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeOc = item.Where(x => x.Mes.Equals("Octubre")).FirstOrDefault();
                        if (existeOc != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeNo = item.Where(x => x.Mes.Equals("Noviembre")).FirstOrDefault();
                        if (existeNo != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }
                        var existeDi = item.Where(x => x.Mes.Equals("Diciembre")).FirstOrDefault();
                        if (existeDi != null)
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            obj.name = exist.anioplan;
                            obj.data.Add(0);
                        }

                        listaMensual.Add(obj);
                        i++;
                    }
                }
            }


            MensualidadDTO mensual = new MensualidadDTO();
            mensual.categorias.Add("Enero");
            mensual.categorias.Add("Febrero");
            mensual.categorias.Add("Marzo");
            mensual.categorias.Add("Abril");
            mensual.categorias.Add("Mayo");
            mensual.categorias.Add("Junio");
            mensual.categorias.Add("Julio");
            mensual.categorias.Add("Agosto");
            mensual.categorias.Add("Septiembre");
            mensual.categorias.Add("Octubre");
            mensual.categorias.Add("Noviembre");
            mensual.categorias.Add("Diciembre");
            mensual.ListaPqMensual = listaMensual;
            return mensual;
        }


        public async Task<ReporteMaterialDetalle> ListarMaterial(RequestDashboradDTO o)
        {
            var resultad = await _context.ReporteMaterialDetalle.FromSqlRaw($"EXEC dashboardMaterial  {o.MaterialId} , {o.PQuinquenalId} , {o.PAnual}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> habilitado = new List<Decimal>();
            foreach (var item in resultad)
            {

                categorias.Add( item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudAprobada + " " + "&nbsp;&nbsp;&nbsp;&nbsp;" + " " + item.Planificado+ "%");
                pendiente.Add(item.longitudPendiente);
                habilitado.Add(item.LongitudAprobada);
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
            var resultad = await _context.ReporteMaterialConstruidaDetalle.FromSqlRaw($"EXEC dashboardMaterialConstruida  {o.MaterialId} , {o.PQuinquenalId} , {o.PAnual}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> construido = new List<Decimal>();
            foreach (var item in resultad)
            {

                categorias.Add(item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudConstruida + " " + "&nbsp;&nbsp;&nbsp;&nbsp;" + " " + item.Planificado+ "%");
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
            var resultad = await _context.DistritosPermisoDTO.FromSqlRaw($"EXEC TOTAL {o.MaterialId}, {o.PQuinquenalId} , {o.PAnual}").ToListAsync();

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
                var norequierecount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 1 , {o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual} ").ToListAsync();
                norequiere.Add(norequierecount.Count);
                var permisodenegadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 2 , {o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual} ").ToListAsync();
                permisodenegado.Add(permisodenegadocount.Count);
                var permisotramitecount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 3 , {o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual} ").ToListAsync();
                permisotramite.Add(permisotramitecount.Count);
                var permisonotramitadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 4 , {o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual}  ").ToListAsync();
                permisonotramitado.Add(permisonotramitadocount.Count);
                var permisootorgadocount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 5 ,{o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual}  ").ToListAsync();
                permisootorgado.Add(permisootorgadocount.Count);
                var sapcount = await _context.DistritosPermisoDTO.FromSqlInterpolated($"EXEC PermisoEstado 6 ,{o.MaterialId}, {item.Descripcion} , {o.PQuinquenalId} , {o.PAnual}  ").ToListAsync();
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
