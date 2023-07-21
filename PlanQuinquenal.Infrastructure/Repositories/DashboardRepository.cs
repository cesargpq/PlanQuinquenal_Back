using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlanQuinquenalContext _context;

        public DashboardRepository(PlanQuinquenalContext context)
        {
            this._context = context;
        }

        public async Task<ResposeDistritosDetalleDTO> ListarAvanceMensual(RequestDashboradDTO o)
        {
            var resultad = await _context.ReporteMaterialDetalle.FromSqlRaw($"EXEC AVANCEMENSUAL  {o.MaterialId} , {o.PQuinquenalId} , {o.PAnual}").ToListAsync();

            return new ResposeDistritosDetalleDTO { };
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
