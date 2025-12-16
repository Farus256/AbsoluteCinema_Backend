using AbsoluteCinema.Infrastructure;
using AbsoluteCinema.Application;
using AbsoluteCinema.Domain;
using System.Text.Json.Serialization;
using AbsoluteCinema.Infrastructure.DbContexts;
using AbsoluteCinema.Infrastructure.Identity.Data;
using AbsoluteCinema.Infrastructure.Seeders;
using AbsoluteCinema.WebAPI.Filters;
using AbsoluteCinema.WebAPI.Grpc.Services;
using Microsoft.AspNetCore.Identity;
using AbsoluteCinema.WebAPI.Swagger;
using Microsoft.OpenApi.Models;
using Grpc.AspNetCore.Server;
using Grpc.AspNetCore.Web;

string reactClientCORSPolicy = "reactClientCORSPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(reactClientCORSPolicy, policy =>
    {
        var clientAddress = builder.Configuration["ClientAddress"]!;
        policy.WithOrigins(clientAddress)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains(); 
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSignalR();
builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = @"Bearer (paste here your token (remove all brackets) )",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        });

    o.OperationFilter<AuthorizeCheckOperationFilter>();

    o.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "AbsoluteCinema API - v1",
        Version = "v1"
    });
});

builder.Services.AddDomainDI();
builder.Services.AddApplicationDI(builder.Configuration);
builder.Services.AddInfrastructureDI(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    
    await TicketStatusSeeder.SeedTicketStatusesAsync(context);
    await RoleSeeder.SeedRolesAsync(roleManager);
    await HallSeeder.SeedHallsAsync(context);
}

// Movie мають бути створені ПЕРЕД Session
await TmdbSeeder.SeedTmdbDataAsync(app.Services);
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Запуск SessionSeeder...");
    await SessionSeeder.SeedSessionsAsync(context, logger);
    logger.LogInformation("SessionSeeder завершено");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(reactClientCORSPolicy);
app.UseGrpcWeb();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AbsoluteCinema.WebAPI.Hubs.RealtimeHub>("/realtimehub");
app.MapGrpcService<MovieGrpcService>().EnableGrpcWeb().RequireCors(reactClientCORSPolicy);

app.Run();
