using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Data
{
    public class PlanQuinquenalContext : DbContext
    {
        public PlanQuinquenalContext()
        {

        }
        public PlanQuinquenalContext(DbContextOptions<PlanQuinquenalContext> options)
            : base(options)
        {
        }
    }
}
