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
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Tokens.JWT.Configuration;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Tokens.JWT.Services;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using MassTransit;

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

// JWT Token Configuration - Must use same secret as IAM Service
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<ITokenService, TokenService>();

// Dependency Injection - Repositories
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

// Dependency Injection - Unit of Work
builder.Services.AddScoped<OsitoPolar.Analytics.Service.Shared.Domain.Repositories.IUnitOfWork, OsitoPolar.Analytics.Service.Shared.Infrastructure.Persistence.EFC.Repositories.UnitOfWork>();

// Dependency Injection - Services
builder.Services.AddScoped<IAnalyticsCommandService, AnalyticsCommandService>();
builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();

// ✅ PHASE 2: HTTP Facades for Microservices Communication
// Analytics Service needs READ-ONLY access to other services for gathering metrics
builder.Services.AddHttpClient<OsitoPolar.Analytics.Service.Shared.Interfaces.ACL.IProfilesContextFacade,
    OsitoPolar.Analytics.Service.Application.ACL.Services.ProfilesHttpFacade>(client =>
{
    var profilesUrl = builder.Configuration["ServiceUrls:ProfilesService"]
        ?? throw new InvalidOperationException("ProfilesService URL not configured");
    client.BaseAddress = new Uri(profilesUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Analytics-Service/1.0");
});

builder.Services.AddHttpClient<OsitoPolar.Analytics.Service.Shared.Interfaces.ACL.IEquipmentContextFacade,
    OsitoPolar.Analytics.Service.Application.ACL.Services.EquipmentHttpFacade>(client =>
{
    var equipmentUrl = builder.Configuration["ServiceUrls:EquipmentService"]
        ?? throw new InvalidOperationException("EquipmentService URL not configured");
    client.BaseAddress = new Uri(equipmentUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Analytics-Service/1.0");
});

builder.Services.AddHttpClient<OsitoPolar.Analytics.Service.Shared.Interfaces.ACL.IWorkOrdersContextFacade,
    OsitoPolar.Analytics.Service.Application.ACL.Services.WorkOrdersHttpFacade>(client =>
{
    var workOrdersUrl = builder.Configuration["ServiceUrls:WorkOrdersService"]
        ?? throw new InvalidOperationException("WorkOrdersService URL not configured");
    client.BaseAddress = new Uri(workOrdersUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Analytics-Service/1.0");
});

builder.Services.AddHttpClient<OsitoPolar.Analytics.Service.Shared.Interfaces.ACL.ISubscriptionsContextFacade,
    OsitoPolar.Analytics.Service.Application.ACL.Services.SubscriptionsHttpFacade>(client =>
{
    var subscriptionsUrl = builder.Configuration["ServiceUrls:SubscriptionsService"]
        ?? throw new InvalidOperationException("SubscriptionsService URL not configured");
    client.BaseAddress = new Uri(subscriptionsUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Analytics-Service/1.0");
});

// ===========================
// MassTransit + RabbitMQ Configuration
// ===========================
builder.Services.AddMassTransit(x =>
{
    // Configure RabbitMQ
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitMqPort = builder.Configuration["RabbitMQ:Port"] ?? "5672";
        var rabbitMqUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitMqPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host($"rabbitmq://{rabbitMqHost}:{rabbitMqPort}", h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        // Configure message retry policy
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

        // Auto-configure all consumers
        cfg.ConfigureEndpoints(context);
    });
});

Console.WriteLine("✅ MassTransit + RabbitMQ configured for Analytics Service");

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
        context.Database.EnsureCreated();
        Console.WriteLine("✅ Database connection successful and schema ensured");
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

// JWT Authorization Middleware - validates tokens and sets HttpContext.Items["User"]
app.UseRequestAuthorization();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Analytics Service running on port 5008");

app.Run();
