using AutoMapper;
using PlanQuinquenal.Core.DTOs.RequestDTO;
using PlanQuinquenal.Core.DTOs.ResponseDTO;
using PlanQuinquenal.Core.Entities;
using PlanQuinquenal.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanQuinquenal.Infrastructure.Repositories
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
          
            CreateMap<PQuinquenalReqDTO, PlanQuinquenal.Core.Entities.PlanQuinquenal>();
            CreateMap<UsuarioRequestDto, Usuario>();
            CreateMap<ProyectoResponseDto, Proyecto>();

            CreateMap<BolsaReemplazo, RequestBolsaDto>();
            CreateMap<RequestBolsaDto, BolsaReemplazo>();


           
                CreateMap<MaestroResponseDto, EstadoAprobacion>();
            CreateMap<EstadoAprobacion, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, CausalReemplazo>();
            CreateMap<CausalReemplazo, MaestroResponseDto>();
            CreateMap<MaestroResponseDto, Material>();
            CreateMap<Material, MaestroResponseDto>();

            CreateMap<ComentarioPY, ComentarioRequestDTO>();
            CreateMap<ComentarioRequestDTO, ComentarioPY>();

            CreateMap<COMENTARIOPQ, ComentarioRequestDTO>();
            CreateMap<ComentarioRequestDTO, COMENTARIOPQ>();

            CreateMap<COMENTARIOPA, ComentarioRequestDTO>();
            CreateMap<ComentarioRequestDTO, COMENTARIOPA>();
            
            CreateMap<MaestroResponseDto, TipoInforme>();
            CreateMap<TipoInforme, MaestroResponseDto>();
            CreateMap<MaestroResponseDto, TipoSeguimiento>();
            CreateMap<TipoSeguimiento, MaestroResponseDto>();




            CreateMap<MaestroResponseDto, TipoPermisosProyecto>();
            CreateMap<TipoPermisosProyecto, MaestroResponseDto>();


            CreateMap<PQuinquenalReqDTO, PQuinquenal>();
            CreateMap<PQuinquenal, PQuinquenalReqDTO>()
                .ForMember(x => x.UsuariosInteresados, c => c.Ignore());


            CreateMap<PQuinquenalReqDTO, PlanAnual>();
            CreateMap<PlanAnual, PQuinquenalReqDTO>()
                .ForMember(x => x.UsuariosInteresados, c => c.Ignore());

            CreateMap<MaestroResponseDto, Distrito>();
            CreateMap<Distrito, MaestroResponseDto>();

            CreateMap<PermisoByIdResponseDto, PermisosProyecto>();
            CreateMap<PermisosProyecto, PermisoByIdResponseDto>();


            CreateMap<MaestroResponseDto, EstadoPermisos>();
            CreateMap<EstadoPermisos, MaestroResponseDto>();
            

            CreateMap<MaestroResponseDto, PlanAnual>();
            CreateMap<PlanAnual, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, PQuinquenal>();
            CreateMap<PQuinquenal, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, TipoProyecto>();
            CreateMap<TipoProyecto, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, TipoRegistro>();
            CreateMap<TipoRegistro, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, Constructor>();
            CreateMap<Constructor, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, EstadoGeneral>();
            
            CreateMap<EstadoGeneral, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, EstadoPQ>();
            CreateMap<EstadoPQ, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, TipoImpedimento>();
            CreateMap<TipoImpedimento, MaestroResponseDto>();

            CreateMap<ImpedimentoUpdateDto, Impedimento>();
            CreateMap<Impedimento, ImpedimentoUpdateDto>();
            

            CreateMap<MaestroResponseDto, ProblematicaReal>();
            CreateMap<ProblematicaReal, MaestroResponseDto>();

            CreateMap<MaestroResponseDto, TipoUsuario>();
            CreateMap<TipoUsuario, MaestroResponseDto>();
            CreateMap<DocumentoRequestDto, DocumentosPy>();
            CreateMap<DocumentoRequestDto, DocumentosPQ>();
            CreateMap<DocumentoRequestDto, DocumentosPA>();


            CreateMap<InformeReqDTO, Informe>()
                .ForMember(x => x.UserInteresados, c => c.Ignore())
                .ForMember(x => x.Participantes, c => c.Ignore())
                .ForMember(x => x.Asistentes, c => c.Ignore())
                .ForMember(x => x.FechaInforme, c => c.Ignore())
                .ForMember(x => x.FechaCompromiso, c => c.Ignore())
                .ForMember(x => x.FechaReunion, c => c.Ignore());
            CreateMap<Informe, InformeReqDTO>();

            CreateMap<DocumentoResponseDto, DocumentosPy>();


            CreateMap<BaremoResponseDTO, Baremo>();


            CreateMap<PQuinquenal, PQuinquenalResponseDto>();
            CreateMap<PQuinquenalResponseDto, PQuinquenal>();               

            CreateMap<DocumentoResponseDto, DocumentosPQ>();
            CreateMap<DocumentoResponseDto, DocumentosPA>();
            CreateMap<DocumentoPermisosResponseDTO, DocumentosPermisos>();
            CreateMap<DocumentosPermisos, DocumentoPermisosResponseDTO>()
                .ForMember(X => X.Fecha, y => y.MapFrom(src => DateOnly(src.Fecha)));



            CreateMap<DocumentosPy, DocumentoResponseDto>()
                .ForMember(x => x.nombreArchivo, y => y.MapFrom(src => src.CodigoDocumento))
                .ForMember(x => x.codigoDocumento, y => y.MapFrom(src => Separa(src.NombreDocumento)))
                .ForMember(x => x.Aprobaciones, y => y.MapFrom(src => DateOnly(src.Aprobaciones)));


            CreateMap<DocumentosPQ, DocumentoResponseDto>()
                .ForMember(x => x.nombreArchivo, y => y.MapFrom(src => src.CodigoDocumento))
                .ForMember(x => x.codigoDocumento, y => y.MapFrom(src => Separa(src.NombreDocumento)))
                .ForMember(x => x.Aprobaciones, y => y.MapFrom(src => DateOnly(src.Aprobaciones)));
            CreateMap<DocumentosPA, DocumentoResponseDto>()
                .ForMember(x => x.nombreArchivo, y => y.MapFrom(src => src.CodigoDocumento))
                .ForMember(x => x.codigoDocumento, y => y.MapFrom(src => Separa(src.NombreDocumento)))
                .ForMember(x => x.Aprobaciones, y => y.MapFrom(src => DateOnly(src.Aprobaciones)));


            CreateMap<UsuariosInteresadosPy, UsuariosInteresadosPyResponseDto>();
            CreateMap<UsuariosInteresadosPyResponseDto, UsuariosInteresadosPy > ();


            CreateMap<UsuariosInteresadosInformesResponseDto, UsuariosInteresadosInformes>();
            CreateMap<UsuariosInteresadosInformes, UsuariosInteresadosInformesResponseDto>();

            CreateMap<UsuariosInteresadosInformesResponseDto, ActaAsistentes>();
            CreateMap<ActaAsistentes, UsuariosInteresadosInformesResponseDto>();


            CreateMap<UsuariosInteresadosInformesResponseDto, ActaParticipantes>();
            CreateMap<ActaParticipantes, UsuariosInteresadosInformesResponseDto>();


            CreateMap<Usuario, UsuarioResponseDto>()
               .ForMember(x => x.Nombre, y => y.MapFrom(src => src.nombre_usu +" "+ src.apellido_usu))
               .ForMember(x => x.Id, y => y.MapFrom(src => src.cod_usu));


            CreateMap<UsuariosInteresadosInformes, ParticipantesResponseDto>();
            CreateMap<ParticipantesResponseDto, UsuariosInteresadosInformes>();


            

                

            CreateMap<PQuinquenal, PQuinquenalResponseDto>()
              .ForMember(x => x.AnioPlan, y => y.MapFrom(src => src.AnioPlan))
              .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));

            CreateMap<PlanAnual, PlanAnualResponseDto>()
              .ForMember(x => x.AnioPlan, y => y.MapFrom(src => src.AnioPlan))
              .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));


            CreateMap<ActaParticipantes, UsuarioResponseDto>();
            CreateMap<UsuarioResponseDto, ActaParticipantes>();

            CreateMap<ActaAsistentes, UsuarioResponseDto>();
            CreateMap<UsuarioResponseDto, ActaAsistentes>();

            CreateMap<UsuariosInteresadosInformes, UsuarioResponseDto>();
            CreateMap<UsuarioResponseDto, UsuariosInteresadosInformes>();


            CreateMap<ProyectoRequestDto, Proyecto>();
            CreateMap<Proyecto, ProyectoRequestDto>();

            CreateMap<Informe, InformeResponseDto>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateOnly(src.FechaCreacion)))
                .ForMember(dest => dest.FechaInforme, opt => opt.MapFrom(src => DateOnly(src.FechaInforme)))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateOnly(src.FechaModificacion)))
                .ForMember(dest => dest.FechaReunion, opt => opt.MapFrom(src => DateOnly(src.FechaReunion)))
                .ForMember(dest => dest.FechaCompromiso, opt => opt.MapFrom(src => DateOnly(src.FechaCompromiso)))
                .ForMember(
                    dest => dest.Participantes, opt => opt.MapFrom(src => src.Participantes))
                .ForMember(
                    dest => dest.Asistentes, opt => opt.MapFrom(src => src.Asistentes))
                .ForMember(
                    dest => dest.UserInteresados, opt => opt.MapFrom(src => src.UserInteresados));

            CreateMap<Proyecto, ProyectoResponseDto>()
                .ForMember(
                    dest => dest.IngenieroResponsables,
                    opt => opt.MapFrom(src => src.IngenieroResponsable)).ForMember(
                    dest => dest.PQuinquenalResponseDto,
                    opt => opt.MapFrom(src => src.PQuinquenal)).ForMember(
                    dest => dest.PlanAnualResponseDto,
                    opt => opt.MapFrom(src => src.PlanAnual)).ForMember(
                    dest => dest.EstadoGeneralDesc,
                    opt => opt.MapFrom(src =>
                    GetStateGeneral(src.LongAprobPa,
                    src.LongRealHab, src.LongRealPend,
                    (src.LongRealPend - src.LongReemplazada),
                    src.LongImpedimentos))).ForMember(
                    dest => dest.longPendienteEjecución,
                    opt => opt.MapFrom(src => src.LongRealPend - src.LongReemplazada))
                    .ForMember(
                    dest => dest.Avance,
                    opt => opt.MapFrom(src => ObtenerAvance(src.LongRealHab, src.LongAprobPa)))
                    .ForMember(dest => dest.FechaGasificacion, opt => opt.MapFrom(src => DateOnly(src.FechaGasificacion)));


            CreateMap<Usuario, UsuarioRequestDto>();

        }

        private Decimal? ObtenerAvance(Decimal? LongRealHab, Decimal? LongAprobPa)
        {
            decimal? dato = 0;
            if (LongAprobPa > 0)
            {
                dato = LongRealHab / LongAprobPa;
            }
            else
            {
                dato = 0;
            }
            return dato;
        }
        private DateTime ParseDateTime(string data)
        {

            
                return DateTime.Parse(data);
            
           
            
        }
        private string DateOnly(DateTime? data)
        {
            
                return data?.ToString("dd/MM/yyyy");
            
           
        }
        private string Separa(string data)
        {
            return data.Split(".")[0];
        }
        private string GetStateGeneral(Decimal? LongAprobPa, Decimal? LongRealHab, Decimal? LongRealPend, Decimal? LongPendEjecucion, Decimal? LongImpedimento)
        {
            
            if(LongRealHab > 0 && LongRealPend > 0 && (Math.Round((decimal)LongPendEjecucion, 0) > 0))
            {
                return Constantes.PARCIALMENTEEJECUTADO;
            }
            if(LongRealHab ==0 && LongRealPend ==0)
            {
                return Constantes.NOEJECUTADO;
            }
            if( (LongRealPend==0 && (LongRealHab-LongAprobPa == 0) && LongPendEjecucion==0) || ( (LongRealHab> LongAprobPa) && LongRealPend == 0 && LongImpedimento==0))
            {
                return Constantes.EJECUTADO;
            }
            if( LongRealPend >0 && Math.Round((decimal)LongPendEjecucion, 0)==0)
            {
                return Constantes.EJECUTADOREEMPLAZO;
            }
            else
            {
                return Constantes.NOOPTION;
            }
            
        }

    }
}
