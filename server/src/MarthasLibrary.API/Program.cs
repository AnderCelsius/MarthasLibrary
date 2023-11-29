using MarthasLibrary.API.Extensions;
using MarthasLibrary.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();
builder.AddFluentValidationExtension();

var app = builder.Build();

if (!app.Environment.IsProduction() && !app.Environment.IsTesting())
{
  await app.Services.MigrateDatabaseToLatestVersion<LibraryDbContext>();
}

app.ConfigurePipeline(builder);

app.Run();
