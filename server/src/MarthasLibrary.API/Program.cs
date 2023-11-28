using MarthasLibrary.API;
using MarthasLibrary.API.Extensions;
using MarthasLibrary.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  await app.Services.MigrateDatabaseToLatestVersion<LibraryDbContext>();
}

app.ConfigurePipeline();

app.Run();
