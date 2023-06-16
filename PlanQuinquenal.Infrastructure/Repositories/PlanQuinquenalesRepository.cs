using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Interfaces;
using PlanQuinquenal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class PlanQuinquenalesRepository : IPlanQuinquenalesRepository
    {
        private readonly PlanQuinquenalContext _context;
        private readonly IMapper mapper;
        public PlanQuinquenalesRepository(PlanQuinquenalContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this._context = context;
        }

        public async Task<bool> CreatePQ(PQuinquenalReqDTO pQuinquenalReqDTO)
        {
            
            var pq = mapper.Map<PlanQuinquenal.Core.Entities.PlanQuinquenal>(pQuinquenalReqDTO);
            _context.Add(pq);
            await _context.SaveChangesAsync();

            var pqFirst = await _context.PlanQuinquenal.Where(x => x.Pq == pQuinquenalReqDTO.Pq).FirstOrDefaultAsync();
            
            List<PQUsuariosInteresados> listPqUser = new List<PQUsuariosInteresados>();
            foreach (var item in pQuinquenalReqDTO.IdUsuario)
            {
                PQUsuariosInteresados pqUser = new PQUsuariosInteresados();
                pqUser.PlanQuinquenalId = pqFirst.Id;
                pqUser.UsuarioId = item;
                pqUser.Estado = true;
                listPqUser.Add(pqUser);
            }
            //listPqUser.ForEach(n => _context.Add(n));
            _context.AddRange(listPqUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PlanQuinquenal.Core.Entities.PlanQuinquenal>> Get()
        {
            var pq = await _context.PlanQuinquenal.ToListAsync();
                        
            return pq;
        }
    }
}
