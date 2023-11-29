using FluentValidation;
using FluentValidation.AspNetCore;
using static MarthasLibrary.API.Features.Books.Create;

namespace MarthasLibrary.API.Extensions
{
  public static class FluentValidationExtension
  {
    public static void AddFluentValidationExtension(this WebApplicationBuilder builder)
    {
      builder.Services.AddFluentValidationAutoValidation()
          .AddFluentValidationClientsideAdapters()
          .AddValidatorsFromAssemblyContaining<CreateBookValidator>();
    }
  }
}
