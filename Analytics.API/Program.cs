using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using OsitoPolar.Analytics.Service.Domain.Repositories;
using OsitoPolar.Analytics.Service.Domain.Services;
using OsitoPolar.Analytics.Service.Application.Internal.CommandServices;
using OsitoPolar.Analytics.Service.Application.Internal.QueryServices;
using OsitoPolar.Analytics.Service.Infrastructure.Persistence.EFC.Configuration;
using OsitoPolar.Analytics.Service.Infrastructure.Persistence.EFC.Repositories;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Interfaces.ASP.Configuration;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
{
    if (connectionString != null)
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
});

// ⚠️ CRÍTICO: Register DbContext as base class for UnitOfWork and BaseRepository
// Sin esto, obtendrás error: "Unable to resolve service for type 'Microsoft.EntityFrameworkCore.DbContext'"
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AnalyticsDbContext>());

// Dependency Injection - Repositories
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

// Dependency Injection - Unit of Work
builder.Services.AddScoped<OsitoPolar.Analytics.Service.Shared.Domain.Repositories.IUnitOfWork, OsitoPolar.Analytics.Service.Shared.Infrastructure.Persistence.EFC.Repositories.UnitOfWork>();

// Dependency Injection - Services
builder.Services.AddScoped<IAnalyticsCommandService, AnalyticsCommandService>();
builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();

// Controllers
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OsitoPolar Analytics Service API",
        Version = "v1",
        Description = "Analytics Microservice - Equipment Analytics & Monitoring"
    });
    options.EnableAnnotations();
});

var app = builder.Build();

// Verify database connection on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AnalyticsDbContext>();
    try
    {
        context.Database.CanConnect();
        Console.WriteLine("✅ Database connection successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database connection failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Analytics Service running on port 5008");

app.Run();
