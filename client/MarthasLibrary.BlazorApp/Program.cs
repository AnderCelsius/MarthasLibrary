using MarthasLibrary.APIClient;
using MarthasLibrary.BlazorApp.Components;
using MarthasLibrary.BlazorApp.Extensions;
using MarthasLibrary.BlazorApp.Services;
using MarthasLibrary.Common.Authorization;
using Polly;
using Polly.Extensions.Http;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<AuthorizationHeaderHandler>();

builder.Services.AddSingleton<BookService>();
builder.Services.AddScoped<AuthenticationService>();
// Configure a retry policy
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // This handles most of the transient errors (5xx, 408, etc.)
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Optionally, handle specific HTTP status codes.
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient<IMarthasLibraryAPIClient, MarthasLibraryAPIClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("BookApiHost").Value!);
}).AddHttpMessageHandler<AuthorizationHeaderHandler>();

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["DuendeISP:Authority"] ?? "https://localhost:5001");
}).AddPolicyHandler(retryPolicy);

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
