using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SistemaHotelAloha.AccesoDatos;
using SistemaHotelAloha.Web.Auth;
using SistemaHotelAloha.Web.Endpoints;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Acceso a datos ---
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en appsettings.json (Web)");
builder.Services.AddScoped(_ => new ReservasAdoRepository(conn));

// --- Blazor / Razor / Controllers ---
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);
builder.Services.AddControllers();

// --- Auth simple + almacenamiento protegido ---
builder.Services.AddAuthorizationCore();
// IMPORTANTÍSIMO: registra todos los servicios de Protected*Storage
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ProtectedSessionStorage>();


builder.Services.AddScoped<SimpleAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthStateProvider>());

// Si usas IHttpContextAccessor para endpoints PDF, etc.
builder.Services.AddHttpContextAccessor();
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// --- Pipeline ---
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
app.MapPdfEndpoints();               
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");      

app.Run();
