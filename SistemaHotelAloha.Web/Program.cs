using SistemaHotelAloha.Servicios;

var builder = WebApplication.CreateBuilder(args);

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
