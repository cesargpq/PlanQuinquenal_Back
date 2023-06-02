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
        public virtual DbSet<Acciones_Rol> Acciones_Rol { get; set; }
        public virtual DbSet<Unidad_negocio> Unidad_negocio { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Proyectos> Proyectos { get; set; }

        public virtual DbSet<TokenAuth> TokenAuth { get; set; }
        public virtual DbSet<DobleFactor> DobleFactor { get; set; }
        
        public virtual DbSet<Baremo> Baremo { get; set; }


        public virtual DbSet<Impedimentos> Impedimentos { get; set; }
        public virtual DbSet<Imped_evi_reempla> Imped_evi_reempla { get; set; }
        public virtual DbSet<Imped_gest_inast> Imped_gest_inast { get; set; }
        public virtual DbSet<Comentarios_proyec> Comentarios_proyec { get; set; }
        public virtual DbSet<Permisos_proyec> Permisos_proyec { get; set; }
        public virtual DbSet<Docum_proyecto> Docum_proyecto { get; set; }
        public virtual DbSet<Informes_actas> Informes_actas { get; set; }
        public virtual DbSet<Notificaciones> Notificaciones { get; set; }
        public virtual DbSet<Config_notificaciones> Config_notificaciones { get; set; }
        public virtual DbSet<UsuariosIntersados_pry> UsuariosIntersados_pry { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Usuario>().ToTable("usuario").
        //        HasKey(x => new { x.cod_usu, x.cod_perfil }).HasRequired(x => x.PersonOne).WithMany().HasForeignKey(x => x.Person1Id);
        //}


    }
}

