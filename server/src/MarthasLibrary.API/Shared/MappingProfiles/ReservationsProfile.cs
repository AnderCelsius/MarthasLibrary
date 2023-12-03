using AutoMapper;
using MarthasLibrary.API.Features.Reservations;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class ReservationsProfile : Profile
{
  public ReservationsProfile()
  {
    CreateMap<Reservation, MakeReservation.Response>()
      .ForCtorParam(nameof(MakeReservation.Response.ReservationId), op => op.MapFrom(x => x.Id));
  }

}