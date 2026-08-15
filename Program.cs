using Microsoft.EntityFrameworkCore;
using VehicleTrackingApi.Data;
using VehicleTrackingApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS : autorise le projet Blazor à appeler l'API
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5047")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est manquante.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<DistanceService>();
builder.Services.AddScoped<ZoneService>();
builder.Services.AddScoped<StatutService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");
app.MapControllers();

app.Run();
