using AutoMapper;
using MarthasLibrary.API.Features.Reservations;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class ReservationProfile : Profile
{
  public ReservationProfile()
  {
    CreateMap<List<Reservation>, GetAll.Response>()
      .ConstructUsing((src, context) => new GetAll.Response(context.Mapper.Map<IReadOnlyCollection<ReservationDetails>>(src)));

    CreateMap<Reservation, ReservationDetails>()
      .ConstructUsing((reservation, context) =>
      {
        // Retrieve the title from the context
        var title = context.Items.TryGetValue("Title", out var bookTitle) ? bookTitle.ToString() : string.Empty;

        // Construct the ReservationDetails object
        return new ReservationDetails(
          reservation.Id,
          reservation.BookId,
          reservation.CustomerId,
          title,
          reservation.ReservedDate,
          reservation.ExpiryDate);
      });
  }
}