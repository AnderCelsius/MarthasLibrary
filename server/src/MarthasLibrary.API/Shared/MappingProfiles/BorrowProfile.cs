using AutoMapper;
using MarthasLibrary.API.Features.Borrow;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class BorrowProfile : Profile
{
  public BorrowProfile()
  {
    CreateMap<Borrow, BorrowDetails>()
      .ForCtorParam(nameof(BorrowDetails.BorrowId), op => op.MapFrom(x => x.Id));

    CreateMap<List<Borrow>, GetAll.Response>()
      .ConstructUsing((src, context) => new GetAll.Response(context.Mapper.Map<IReadOnlyCollection<BorrowDetails>>(src)));
  }
}