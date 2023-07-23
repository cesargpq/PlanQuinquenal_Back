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

        public async Task<ResposeDistritosDetalleDTO> ListarAvanceMensual(AvanceMensualDto o)
        {
            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIO  {o.Anio} , {o.MaterialId}").ToListAsync();

            var data = resultad.GroupBy(x=>x.pquinquenalId).ToList();

            List<Decimal> enero = new List<Decimal>();
            List<Decimal> febrero = new List<Decimal>();
            List<Decimal> marzo = new List<Decimal>();
            List<Decimal> abril = new List<Decimal>();
            List<Decimal> mayo = new List<Decimal>();
            List<Decimal> junio = new List<Decimal>();
            List<Decimal> julio = new List<Decimal>();
            List<Decimal> agosto = new List<Decimal>();
            List<Decimal> setiembre = new List<Decimal>();
            List<Decimal> octubre = new List<Decimal>();
            List<Decimal> noviembre = new List<Decimal>();
            List<Decimal> diciembre = new List<Decimal>();
            foreach (var item in data)
            {
                foreach (var planes in item)
                {

                    if (planes.Mes.Equals("Enero"))
                    {
                        enero.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        enero.Add(0);
                    }
                    if (planes.Mes.Equals("Febrero"))
                    {
                        febrero.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        febrero.Add(0);

                    }
                    if (planes.Mes.Equals("Marzo"))
                    {
                        marzo.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        marzo.Add(0);
                    }
                    if (planes.Mes.Equals("Abril"))
                    {
                        abril.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        abril.Add(0);
                    }
                    if (planes.Mes.Equals("Mayo"))
                    {
                        mayo.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        mayo.Add(0);
                    }
                    if (planes.Mes.Equals("Junio"))
                    {
                        junio.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        junio.Add(0);
                    }
                    if (planes.Mes.Equals("Julio"))
                    {
                        julio.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        julio.Add(0);
                    }
                    if (planes.Mes.Equals("Agosto"))
                    {
                        agosto.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        agosto.Add(0);
                    }
                    if (planes.Mes.Equals("Septiembre"))
                    {
                        setiembre.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        setiembre.Add(0);
                    }
                    if (planes.Mes.Equals("Octubre"))
                    {
                        octubre.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        octubre.Add(0);
                    }
                    if (planes.Mes.Equals("Noviembre"))
                    {
                        noviembre.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        noviembre.Add(0);
                    }
                    if (planes.Mes.Equals("Diciembre"))
                    {
                        diciembre.Add(planes.LongitudConstruida);
                    }
                    else
                    {
                        diciembre.Add(0);
                    }

                }
                
            }

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
