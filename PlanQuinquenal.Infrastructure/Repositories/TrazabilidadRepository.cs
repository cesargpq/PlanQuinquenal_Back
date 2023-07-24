using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Core.Utilities;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class TrazabilidadRepository : ITrazabilidadRepository
    {
        private readonly PlanQuinquenalContext _context;

        public TrazabilidadRepository(PlanQuinquenalContext context)
        {
            this._context = context;
        }
        public async Task<ResponseDTO> Add(List<Trazabilidad> t)
        {
			try
			{

                await _context.BulkInsertAsync(t);
                
                return new ResponseDTO
                {
                    Message = Constantes.CreacionExistosa,
                    Valid = true
                };
            }
			catch (Exception)
			{

                return new ResponseDTO
                {
                    Message = Constantes.ErrorSistema,
                    Valid = false
                };
            }
        }

        public async  Task<PaginacionResponseDtoException<TrazabilidadDetalle>> Listar(RequestAuditDto r)
        {
            var resultad = await _context.TrazabilidadDetalle.FromSqlInterpolated($"EXEC listartrazabilidad  {r.Pagina} , {r.RecordsPorPagina}").ToListAsync();

            var dato = new PaginacionResponseDtoException<TrazabilidadDetalle>
            {
                Cantidad = resultad.Count(),
                Model  = resultad
            };
            return dato;
        }
    }
}
