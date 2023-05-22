﻿using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<TablaPerm_viz_modulo> Perm_viz_modulo { get; set; }
        public virtual DbSet<TablaPermisos_viz_seccion> Permisos_viz_seccion { get; set; }
        public virtual DbSet<Secciones> Secciones { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Secc_modulos> Secc_modulos { get; set; }
        public virtual DbSet<ColumTablaUsu> ColumTablaUsu { get; set; }
        public virtual DbSet<TablasAuditoria> TablasAuditoria { get; set; }
        public virtual DbSet<EventosAuditoria> EventosAuditoria { get; set; }
        public virtual DbSet<PlanQuinquenal.Core.Entities.PlanQuinquenal> PlanQuinquenal { get; set; }
        public virtual DbSet<PQUsuariosInteresados> PQUsuariosInteresados { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Usuario>().ToTable("usuario").
        //        HasKey(x => new { x.cod_usu, x.cod_perfil }).HasRequired(x => x.PersonOne).WithMany().HasForeignKey(x => x.Person1Id);
        //}

    }
}

