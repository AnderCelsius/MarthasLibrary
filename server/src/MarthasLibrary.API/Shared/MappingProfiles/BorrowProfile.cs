using AutoMapper;
using MarthasLibrary.API.Features.Borrow;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class BorrowProfile : Profile
{
  public BorrowProfile()
  {
    CreateMap<Borrow, BorrowBook.Response>()
      .ForCtorParam(nameof(BorrowBook.Response.BorrowId), op => op.MapFrom(x => x.Id));
  }
}