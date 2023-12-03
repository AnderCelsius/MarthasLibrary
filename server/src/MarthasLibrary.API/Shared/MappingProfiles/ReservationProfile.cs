using AutoMapper;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class ReservationProfile : Profile
{
  public ReservationProfile()
  {
    CreateMap<Reservation, ReservationDetails>()
      .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Id))
      .ForMember(dest => dest.Title,
        opt => opt.MapFrom((_, _, _, context) =>
          context.Items["Title"] as string ?? string.Empty));
  }
}