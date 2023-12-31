﻿using AutoMapper;
using MarthasLibrary.API.Features.Books;
using MarthasLibrary.Core.Entities;

namespace MarthasLibrary.API.Shared.MappingProfiles;

public class BookProfile : Profile
{
  public BookProfile()
  {
    // Mapping from the Book entity to the GetAll.Response.Book record.
    CreateMap<Book, BookDetails>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
      .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
      .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
      .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => src.Isbn))
      .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
      .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate));

    CreateMap<Book, Create.Response>()
      .ForCtorParam(nameof(Create.Response.Id), op => op.MapFrom(x => x.Id));

    CreateMap<Book, GetById.Response>()
      .ConstructUsing((src, context) => new GetById.Response(context.Mapper.Map<BookDetails>(src)));
  }
}
