using AutoMapper;
using MarthasLibrary.API.Features.Borrow;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class BorrowProfile : Profile
{
  public BorrowProfile()
  {
    CreateMap<List<Borrow>, GetAll.Response>()
      .ConstructUsing((src, context) => new GetAll.Response(context.Mapper.Map<IReadOnlyCollection<BorrowDetails>>(src)));

    CreateMap<Borrow, BorrowDetails>()
      .ConstructUsing((borrow, context) =>
      {
        // Retrieve the title from the context
        var title = context.Items.TryGetValue("Title", out var bookTitle) ? bookTitle.ToString() : string.Empty;

        // Construct the ReservationDetails object
        return new BorrowDetails(
          borrow.Id,
          borrow.CustomerId,
          borrow.BookId,
          title,
          borrow.BorrowDate,
          borrow.DueDate);
      });
  }
}