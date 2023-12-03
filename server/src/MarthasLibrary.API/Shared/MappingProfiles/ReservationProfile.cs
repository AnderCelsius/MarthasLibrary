using AutoMapper;
using MarthasLibrary.API.Features.Reservations;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class ReservationProfile : Profile
{
  public ReservationProfile()
  {
    CreateMap<Reservation, MakeReservation.Response>()
      .ForCtorParam(nameof(MakeReservation.Response.ReservationId), op => op.MapFrom(x => x.Id));

    CreateMap<Reservation, ReservationDetails>()
      .ForMember(dest => dest.Title,
        opt => opt.MapFrom((_, _, _, context) =>
          context.Items["Title"] as string ?? string.Empty));
  }
}