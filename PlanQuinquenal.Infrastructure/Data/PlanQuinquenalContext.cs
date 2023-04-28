using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.Entities;
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

        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<TablaLogicaDatos> TablaLogicaDatos { get; set; }
        public virtual DbSet<TablaLogica> TablaLogica { get; set; }

    }
}

