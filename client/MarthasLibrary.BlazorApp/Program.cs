using MarthasLibrary.APIClient;
using MarthasLibrary.BlazorApp.Components;
using MarthasLibrary.BlazorApp.Extensions;
using MarthasLibrary.Common.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
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
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Run();
