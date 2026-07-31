using AutoMapper;
using Osint.Application.DTOs;
using Osint.Domain.Entities;

namespace Osint.Mapper.Profiles;

public class BusquedaProfile : Profile
{
    public BusquedaProfile()
    {
        CreateMap<Senal, SenalDto>(MemberList.Source);
        CreateMap<Busqueda, BusquedaResponseDto>(MemberList.Source)
            .ForMember(d => d.busqueda_id, opt => opt.MapFrom(s => s.busqueda_id.ToString()))
            .ForSourceMember(s => s.lote_id, opt => opt.DoNotValidate());
    }
}
