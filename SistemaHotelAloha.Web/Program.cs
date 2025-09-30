using SistemaHotelAloha.Web.Services;
using SistemaHotelAloha.Web.Security;
using SistemaHotelAloha.Web.Data;
using SistemaHotelAloha.Web.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using SistemaHotelAloha.Servicios;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<Db>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// --- UI hoteles estilo hoteles.com ---
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddSingleton<IHotelRepository, InMemoryHotelRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
// --- fin UI hoteles ---

builder.Services.AddServerSideBlazor();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ClienteService>();
builder.Services.AddSingleton<HabitacionService>();
builder.Services.AddSingleton<ReservaService>();
builder.Services.AddSingleton<PagoService>();
builder.Services.AddSingleton<RecepcionistaService>();
builder.Services.AddSingleton<ServicioAdicionalService>();
builder.Services.AddSingleton<TipoHabitacionService>();
builder.Services.AddSingleton<ReservaServicioService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
