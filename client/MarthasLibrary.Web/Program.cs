using MarthasLibrary.APIClient;
using MarthasLibrary.Common.Authorization;
using MarthasLibrary.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<AuthorizationHeaderHandler>();

builder.Services.AddHttpClient<IMarthasLibraryAPIClient, MarthasLibraryAPIClient>(client =>
{
  client.BaseAddress = new Uri(builder.Configuration.GetSection("BookApiHost").Value!);
}).AddHttpMessageHandler<AuthorizationHeaderHandler>();

builder.Services.AddHttpClient("IDPClient", client =>
{
  client.BaseAddress = new Uri(builder.Configuration["DuendeISP:Authority"] ?? "https://localhost:5001");
});

builder.AddOpenIdConnectAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
