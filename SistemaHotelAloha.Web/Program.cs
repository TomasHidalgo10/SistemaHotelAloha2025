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

// Auth simple + storage protegido
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ProtectedLocalStorage>();

// 👇 Registramos el provider COMO SÍ MISMO y como base interface, apuntando a la MISMA instancia
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

app.MapPdfEndpoints(); // 👈 agrega el endpoint /pdf/reserva/{id}

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

// Pipeline
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
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
