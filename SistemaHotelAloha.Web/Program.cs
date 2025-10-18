using SistemaHotelAloha.Web.Services;
using SistemaHotelAloha.Web.Security;
using SistemaHotelAloha.Web.Data;
using SistemaHotelAloha.Web.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using SistemaHotelAloha.Servicios;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// -------------------- EF Core MySQL --------------------
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AlohaDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

builder.Services.AddScoped<UserRepository>();

// -------------------- Blazor + MVC --------------------
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>();
builder.Services.AddSingleton<IHotelRepository, InMemoryHotelRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHasher>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

