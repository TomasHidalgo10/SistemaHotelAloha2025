using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Web.Auth;
using SistemaHotelAloha.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ReservasAdoRepository>();
// Blazor
builder.Services.AddRazorPages();
builder.Services
    .AddServerSideBlazor()
    .AddCircuitOptions(o => o.DetailedErrors = true);

// Autenticación “simple” para Blazor
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<SimpleAuthStateProvider>(); // <-- el tuyo
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthStateProvider>());

// Controllers (PDF)
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Repo ADO.NET con cadena válida
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en appsettings.json del proyecto Web");
builder.Services.AddScoped(_ => new ReservasAdoRepository(conn));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();