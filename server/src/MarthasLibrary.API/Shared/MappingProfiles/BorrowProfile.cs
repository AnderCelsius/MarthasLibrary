using AutoMapper;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class BorrowProfile : Profile
{
  public BorrowProfile()
  {
    CreateMap<Borrow, BorrowDetails>()
      .ForCtorParam(nameof(BorrowDetails.BorrowId), op => op.MapFrom(x => x.Id));
  }
}