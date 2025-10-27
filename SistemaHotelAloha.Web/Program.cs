global using SistemaHotelAloha.AccesoDatos;
global using SistemaHotelAloha.AccesoDatos.Infra;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SistemaHotelAloha.Web.Auth;


var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Autenticación “simple” para Blazor
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<SimpleAuthStateProvider>(); // <-- el tuyo
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthStateProvider>());

// tus repos ADO que ya tenías:
builder.Services.AddScoped<SistemaHotelAloha.AccesoDatos.UsuarioAdoRepository>();
builder.Services.AddScoped<SistemaHotelAloha.AccesoDatos.HabitacionAdoRepository>();
builder.Services.AddScoped<SistemaHotelAloha.AccesoDatos.ReservasAdoRepository>();
builder.Services.AddScoped<SistemaHotelAloha.AccesoDatos.ServicioAdicionalAdoRepository>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>();
builder.Services.AddScoped<SimpleAuthStateProvider>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();