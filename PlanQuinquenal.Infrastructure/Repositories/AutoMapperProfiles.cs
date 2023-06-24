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


            
            CreateMap<Usuario, UsuarioResponseDto>()
               .ForMember(x => x.Nombre, y => y.MapFrom(src => src.nombre_usu +" "+ src.apellido_usu))
               .ForMember(x => x.Id, y => y.MapFrom(src => src.cod_usu));

            CreateMap<PQuinquenal, PQuinquenalResponseDto>()
              .ForMember(x => x.AnioPlan, y => y.MapFrom(src => src.AnioPlan))
              .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));

            CreateMap<PlanAnual, PlanAnualResponseDto>()
              .ForMember(x => x.AnioPlan, y => y.MapFrom(src => src.AnioPlan))
              .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));

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
                    opt => opt.MapFrom(src => src.LongRealPend - src.LongReemplazada));



            CreateMap<Usuario, UsuarioRequestDto>();

        }
        private string GetStateGeneral(Decimal LongAprobPa, Decimal LongRealHab, Decimal LongRealPend, Decimal LongPendEjecucion, Decimal LongImpedimento)
        {
            if(LongRealHab > 0 && LongRealPend > 0 && (Math.Round(LongPendEjecucion, 0) > 0))
            {
                return Constantes.PARCIALMENTEEJECUTADO;
            }
            if(LongRealHab ==0 && LongRealPend ==0)
            {
                return Constantes.NOEJECUTADO;
            }
            if((LongRealPend==0 && (LongRealHab-LongAprobPa == 0) && LongPendEjecucion==0) || ( (LongRealHab> LongAprobPa) && LongRealPend == 0 && LongImpedimento==0))
            {
                return Constantes.EJECUTADO;
            }
            if( LongRealPend >0 && Math.Round(LongPendEjecucion,0)==0)
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
