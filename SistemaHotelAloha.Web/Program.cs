using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Web.Auth;
using SistemaHotelAloha.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Servicios
// -------------------------
builder.Services.AddScoped<ReservasAdoRepository>();

// Blazor
builder.Services.AddRazorPages();
builder.Services
    .AddServerSideBlazor()
    .AddCircuitOptions(o => o.DetailedErrors = true);

// Auth (estado en Blazor) + storage protegido
builder.Services.AddAuthorizationCore();      // para <AuthorizeView/> en Blazor
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ProtectedLocalStorage>();

// Registramos el provider como sí mismo y como AuthenticationStateProvider (misma instancia)
builder.Services.AddScoped<SimpleAuthStateProvider>();
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

// -------------------------
// Pipeline
// -------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// **CLAVE**: servir archivos estáticos (wwwroot/img, css, js) ANTES de UseRouting
app.UseStaticFiles();

app.UseRouting();

// Si en el futuro protegés endpoints MVC con [Authorize], podés habilitar:
// app.UseAuthentication();
// app.UseAuthorization();

// -------------------------
// Endpoints
// -------------------------
app.MapControllers();                 // para /pdf/... y otros controllers
app.MapPdfEndpoints();                // tu extensión: /pdf/reserva/{id}
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// ÚNICO app.Run()
app.Run();
