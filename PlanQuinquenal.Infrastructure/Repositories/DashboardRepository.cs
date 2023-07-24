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

        public async Task<LongMesesResponseDto> ListarAvanceMensual(AvanceMensualDto o)
        {
            var resultad = await _context.MensualDtoResponse.FromSqlRaw($"EXEC AVANCEMENSUALXANIO  {o.Anio} , {o.MaterialId}").ToListAsync();

            var data = resultad.GroupBy(x=>x.pquinquenalId).ToList();

            var countQuinquenal = await _repositoryMantenedores.GetAllByAttribute("PlanQuinquenal");

            
            List<MensualDtoResponse> listaMensual = new List<MensualDtoResponse>();

            List<string> categorias = new List<string>();
            #region categorias
            categorias.Add("Enero");
            categorias.Add("Febrero");
            categorias.Add("Marzo");
            categorias.Add("Abril");
            categorias.Add("Mayo");
            categorias.Add("Junio");
            categorias.Add("Julio");
            categorias.Add("Agosto");
            categorias.Add("Septiembre");
            categorias.Add("Octubre");
            categorias.Add("Novimebre");
            categorias.Add("Diciembre");
            #endregion  

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

            for (int i = 0; i < countQuinquenal.Count(); i++)
            {
                foreach (var item in data)
                {
                    var exist = item.Where(x => x.pquinquenalId == countQuinquenal.ElementAt(i).Id).FirstOrDefault();
                    if (exist != null && exist.pquinquenalId == countQuinquenal.ElementAt(i).Id)
                    {
                        
                        
                        var existeE = item.Where(x => x.Mes.Equals("Enero")).FirstOrDefault();
                        if (existeE != null)
                        {
                            enero.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            enero.Add(0);
                        }
                        var existeF = item.Where(x => x.Mes.Equals("Febrero")).FirstOrDefault();
                        if (existeF != null)
                        {
                            febrero.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            febrero.Add(0);
                        }
                        var existeM = item.Where(x => x.Mes.Equals("Marzo")).FirstOrDefault();
                        if (existeF != null)
                        {
                            marzo.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            marzo.Add(0);
                        }
                        var existeA = item.Where(x => x.Mes.Equals("Abril")).FirstOrDefault();
                        if (existeF != null)
                        {
                            abril.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            abril.Add(0);
                        }
                        var existeMa = item.Where(x => x.Mes.Equals("Mayo")).FirstOrDefault();
                        if (existeMa != null)
                        {
                            mayo.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            mayo.Add(0);
                        }
                        var existeJ = item.Where(x => x.Mes.Equals("Junio")).FirstOrDefault();
                        if (existeJ != null)
                        {
                            junio.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            junio.Add(0);
                        }
                        var existejul = item.Where(x => x.Mes.Equals("Julio")).FirstOrDefault();
                        if (existejul != null)
                        {
                            julio.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            julio.Add(0);
                        }
                        var existeag = item.Where(x => x.Mes.Equals("Agosto")).FirstOrDefault();
                        if (existeag != null)
                        {
                            agosto.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            agosto.Add(0);
                        }
                        var existes = item.Where(x => x.Mes.Equals("Septiembre")).FirstOrDefault();
                        if (existes != null)
                        {
                            setiembre.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            setiembre.Add(0);
                        }
                        var existeo = item.Where(x => x.Mes.Equals("Octubre")).FirstOrDefault();
                        if (existeo != null)
                        {
                            octubre.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            octubre.Add(0);
                        }
                        var existeN = item.Where(x => x.Mes.Equals("Noviembre")).FirstOrDefault();
                        if (existeN != null)
                        {
                            noviembre.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            noviembre.Add(0);
                        }
                        var existeD = item.Where(x => x.Mes.Equals("Diciembre")).FirstOrDefault();
                        if (existeD != null)
                        {
                            diciembre.Add(exist.LongitudConstruida);
                        }
                        else
                        {
                            diciembre.Add(0);
                        }

                    }
                    else
                    {
                        
                        var datao = countQuinquenal.ElementAt(i).Id;
                        enero.Add(0);
                        febrero.Add(0);
                        marzo.Add(0);
                        abril.Add(0);
                        mayo.Add(0);
                        junio.Add(0);
                        julio.Add(0);
                        agosto.Add(0);
                        setiembre.Add(0);
                        octubre.Add(0);
                        noviembre.Add(0);
                        diciembre.Add(0);
                       
                    }
                    if (i < countQuinquenal.Count()-1)
                    {
                        i++;
                    }

                }

            }
                
            var result = new LongMesesResponseDto 
            { 
                categorias= categorias,
                enero   = enero,
                febrero = febrero,
                marzo = marzo,
                abril   = abril,
                mayo = mayo,
                junio = junio,
                julio = julio,
                agosto = agosto,
                septiembre =setiembre,
                octubre = octubre,
                noviembre =noviembre,
                diciembre = diciembre
            };

            return result;
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
