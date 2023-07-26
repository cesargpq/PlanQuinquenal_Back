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

        public async Task<List<ListaPqMensual>> ListarAvanceMensual(AvanceMensualDto o)
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
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }   
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeF = item.Where(x => x.Mes.Equals("Febrero")).FirstOrDefault();
                        if (existeF != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeM = item.Where(x => x.Mes.Equals("Marzo")).FirstOrDefault();
                        if (existeM != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeA = item.Where(x => x.Mes.Equals("Abril")).FirstOrDefault();
                        if (existeA != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeMay = item.Where(x => x.Mes.Equals("Mayo")).FirstOrDefault();
                        if (existeMay != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeJuni = item.Where(x => x.Mes.Equals("Junio")).FirstOrDefault();
                        if (existeJuni != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeJuli = item.Where(x => x.Mes.Equals("Julio")).FirstOrDefault();
                        if (existeJuli != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeAg = item.Where(x => x.Mes.Equals("Agosto")).FirstOrDefault();
                        if (existeAg != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeSep = item.Where(x => x.Mes.Equals("Septiembre")).FirstOrDefault();
                        if (existeSep != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeOc = item.Where(x => x.Mes.Equals("Octubre")).FirstOrDefault();
                        if (existeOc != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeNo = item.Where(x => x.Mes.Equals("Noviembre")).FirstOrDefault();
                        if (existeNo != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        var existeDi = item.Where(x => x.Mes.Equals("Diciembre")).FirstOrDefault();
                        if (existeDi != null)
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                            name.name = exist.anioplan;
                            obj.PlanQuinquenal = name;
                            obj.Habilitado.data.Add(0);
                        }
                        
                    }
                    else
                    {

                        PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal name = new PlanQuinquenal.Core.DTOs.ResponseDTO.PlanQuinquenal();
                        name.name = countQuinquenal.ElementAt(i).Descripcion;
                        obj.PlanQuinquenal = name;
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);
                        obj.Habilitado.data.Add(0);

                    }
                    if (i < countQuinquenal.Count() - 1)
                    {
                        i++;
                    }
                  
                    listaMensual.Add(obj);

                }
            }

            return listaMensual;
        }

        public async Task<ReporteMaterialDetalle> ListarMaterial(RequestDashboradDTO o)
        {
            var resultad = await _context.ReporteMaterialDetalle.FromSqlRaw($"EXEC dashboardMaterial  {o.MaterialId} , {o.PQuinquenalId} , {o.PAnual}").ToListAsync();

            List<string> categorias = new List<string>();
            List<Decimal> pendiente = new List<Decimal>();
            List<Decimal> habilitado = new List<Decimal>();
            foreach (var item in resultad)
            {

                categorias.Add( item.Distrito + " " + "&nbsp;&nbsp;&nbsp;&nbsp; " + item.LongitudAprobada + " " + "&nbsp;&nbsp;&nbsp;&nbsp;" + " " + item.Planificado);
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
