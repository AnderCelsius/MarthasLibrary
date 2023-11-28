using AutoMapper;
using MarthasLibrary.API.Features.Books;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.SharedResponse.MappingProfiles;

public class BooksProfile : Profile
{
  public BooksProfile()
  {
    // Mapping from the Book entity to the GetAll.Response.Book record.
    CreateMap<Book, BookDetails>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
      .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
      .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
      .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => src.Isbn))
      .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate));

    // Mapping from a collection of Book entities to the GetAll.Response record.
    CreateMap<List<Book>, GetAll.Response>()
      .ConstructUsing((src, context) => new GetAll.Response(context.Mapper.Map<IReadOnlyCollection<BookDetails>>(src)));

    CreateMap<List<Book>, Search.Response>()
      .ConstructUsing((src, context) => new Search.Response(context.Mapper.Map<IReadOnlyCollection<BookDetails>>(src)));

    CreateMap<Book, Create.Response>()
      .ForCtorParam(nameof(Create.Response.Id), op => op.MapFrom(x => x.Id));

    CreateMap<Book, GetById.Response>()
      .ConstructUsing((src, context) => new GetById.Response(context.Mapper.Map<BookDetails>(src)));
  }
}
