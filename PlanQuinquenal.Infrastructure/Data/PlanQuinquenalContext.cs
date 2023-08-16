using Microsoft.EntityFrameworkCore;
using PlanQuinquenal.Core.DTOs;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        
        public virtual DbSet<AvancePQDto> AvancePQDto { get; set; }
        public virtual DbSet<TrazabilidadDetalle> TrazabilidadDetalle { get; set; }
        public virtual DbSet<ReemplazoMaxDTO> ReemplazoMaxDTO { get; set; }
        public virtual DbSet<MensualDtoResponse> MensualDtoResponse { get; set; }
        public virtual DbSet<TrazabilidadVerifica> TrazabilidadVerifica { get; set; }
        public virtual DbSet<ProyectoEtapaResponseDto> ProyectoEtapaResponseDto { get; set; }
        
        public virtual DbSet<ReporteMaterialDetalle> ReporteMaterialDetalle { get; set; }
        public virtual DbSet<Trazabilidad> Trazabilidad { get; set; }
        
        public virtual DbSet<DistritosPermisoDTO> DistritosPermisoDTO { get; set; }
        
        public virtual DbSet<UsuariosInteresadosPQ> UsuariosInteresadosPQ { get; set; }
        public virtual DbSet<ProyectoDetalle> ProyectoDetalle { get; set; }
        public virtual DbSet<ProyectoResponseIdDTO> ProyectoResponseIdDTO { get; set; }
        public virtual DbSet<ZonaPermiso> ZonaPermiso { get; set; }
        public virtual DbSet<BolsaDetalle> BolsaDetalle { get; set; }
        public virtual DbSet<EstadoAprobacion> EstadoAprobacion { get; set; }
        public virtual DbSet<DocumentosBR> DocumentosBR { get; set; }
        public virtual DbSet<Reemplazo> Reemplazo { get; set; }
        public virtual DbSet<BolsaDetalleById> BolsaDetalleById { get; set; }
        
        public virtual DbSet<CausalReemplazo> CausalReemplazo { get; set; }
        public virtual DbSet<ImpedimentoDetalleById> ImpedimentoDetalleById { get; set; }
        public virtual DbSet<ImpedimentoDetalle> ImpedimentoDetalle { get; set; }
        public virtual DbSet<ValidacionLegal> ValidacionLegal { get; set; }
        
        public virtual DbSet<DocumentosImpedimento> DocumentosImpedimento { get; set; }
        public virtual DbSet<BolsaReemplazo> BolsaReemplazo { get; set; }
        public virtual DbSet<ProyectoMasivoDetalle> ProyectoMasivoDetalle { get; set; }
        public virtual DbSet<PQuinquenalResponseDto> PQuinquenalResponseDto { get; set; }
        public virtual DbSet<PQuinquenalResponseDtoID> PQuinquenalResponseDtoID { get; set; }
        
        public virtual DbSet<UsuariosInteresadosPA> UsuariosInteresadosPA { get; set; }
        public virtual DbSet<Informe> Informe { get; set; }
        public virtual DbSet<ComentarioPY> ComentarioPY { get; set; }
        public virtual DbSet<COMENTARIOPQ> COMENTARIOPQ { get; set; }
        public virtual DbSet<COMENTARIOPA> COMENTARIOPA { get; set; }
        public virtual DbSet<COMENTARIOBR> COMENTARIOBR { get; set; }
        public virtual DbSet<TablaConstantes> Constantes { get; set; }

        public virtual DbSet<TipoInforme> TipoInforme { get; set; }
        public virtual DbSet<TipoSeguimiento> TipoSeguimiento { get; set; }
        public virtual DbSet<ActaDinamica> ActaDinamica { get; set; }
        
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<ProblematicaReal> ProblematicaReal { get; set; }
        public virtual DbSet<ActaParticipantes> ActaParticipantes { get; set; }
        public virtual DbSet<ActaAsistentes> ActaAsistentes { get; set; }
        public virtual DbSet<EstadoPermisos> EstadoPermisos { get; set; }
        public virtual DbSet<PermisosProyecto> PermisosProyecto { get; set; }
        public virtual DbSet<TipoPermisosProyecto> TipoPermisosProyecto { get; set; }
        public virtual DbSet<DocumentosPermisos> DocumentosPermisos { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }
        public virtual DbSet<UsuariosInteresadosPy> UsuariosInteresadosPy { get; set; }
        public virtual DbSet<DocumentosPy> DocumentosPy { get; set; }
        public virtual DbSet<DocumentosPQ> DocumentosPQ { get; set; }
        public virtual DbSet<DocumentosPA> DocumentosPA { get; set; }
        public virtual DbSet<UsuariosInteresadosInformes> UsuariosInteresadosInformes { get; set; }
        
        public virtual DbSet<EstadoPQ> EstadoPQ { get; set; }
        public virtual DbSet<TipoImpedimento> TipoImpedimento { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<PQuinquenal> PQuinquenal { get; set; }

        public virtual DbSet<PlanAnual> PlanAnual { get; set; }
        public virtual DbSet<Distrito> Distrito { get; set; }
        public virtual DbSet<TipoProyecto> TipoProyecto { get; set; }
        public virtual DbSet<TipoRegistro> TipoRegistro { get; set; }
        public virtual DbSet<Constructor> Constructor { get; set; }
        public virtual DbSet<EstadoGeneral> EstadoGeneral { get; set; }
        public virtual DbSet<ProyectosP> ProyectosP { get; set; }
        public virtual DbSet<Proyecto> Proyecto { get; set; }
        public virtual DbSet<TablaLogicaDatos> TablaLogicaDatos { get; set; }
        public virtual DbSet<TablaLogica> TablaLogica { get; set; }
        public virtual DbSet<TablaPerm_viz_modulo> Perm_viz_modulo { get; set; }
        public virtual DbSet<TablaPermisos_viz_seccion> Permisos_viz_seccion { get; set; }
        public virtual DbSet<Secciones> Secciones { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Secc_modulos> Secc_modulos { get; set; }
        public virtual DbSet<ColumTablaUsu> ColumTablaUsu { get; set; }
        public virtual DbSet<ColumnasTablas> ColumnasTablas { get; set; }
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

         public virtual DbSet<Impedimento> Impedimento { get; set; }
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
        public virtual DbSet<Informes> Informes { get; set; }
        public virtual DbSet<Actas> Actas { get; set; }
        public virtual DbSet<UsuariosIntersados_Inf> UsuariosIntersados_Inf { get; set; }
        public virtual DbSet<UsuAsisten_Acta> UsuAsisten_Acta { get; set; }
        public virtual DbSet<UsuParticip_Acta> UsuParticip_Acta { get; set; }
        public virtual DbSet<CamposDinam_Acta> CamposDinam_Acta { get; set; }
        public virtual DbSet<Email_modificacion> Email_modificacion { get; set; }
        public virtual DbSet<CorreoTabla> CorreoTabla { get; set; }
        public virtual DbSet<PQComentarios> PQComentarios { get; set; }
        public virtual DbSet<PQDocumentos> PQDocumentos { get; set; }
        public virtual DbSet<CamposModulo_Permisos> CamposModulo_Permisos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<ProyectoDetalle>(
                eb =>
                {
                    eb.HasNoKey();
                });
            modelbuilder.Entity<ImpedimentoDetalle>(
                eb =>
                {
                    eb.HasNoKey();
                });
            modelbuilder.Entity<ProyectoMasivoDetalle>(
                eb =>
                {
                    eb.HasNoKey();
                });
            
                modelbuilder.Entity<ReporteMaterialDetalle>(
                eb =>
                {
                    eb.HasNoKey();
                });
            modelbuilder.Entity<DistritosPermisoDTO>(
                eb =>
                {
                    eb.HasNoKey();
                });
            modelbuilder.Entity<TrazabilidadVerifica>(
               eb =>
               {
                   eb.HasNoKey();
               });
            modelbuilder.Entity<ProyectoEtapaResponseDto>(
               eb =>
               {
                   eb.HasNoKey();
               });
            modelbuilder.Entity<MensualDtoResponse>(
               eb =>
               {
                   eb.HasNoKey();
               });
            modelbuilder.Entity<ReemplazoMaxDTO>(
              eb =>
              {
                  eb.HasNoKey();
              });
            modelbuilder.Entity<TrazabilidadDetalle>(
              eb =>
              {
                  eb.HasNoKey();
              });
            modelbuilder.Entity<AvancePQDto>(
              eb =>
              {
                  eb.HasNoKey();
              });
            

        }


    }
}

