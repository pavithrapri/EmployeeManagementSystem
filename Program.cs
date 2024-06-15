using DemoEmployeeDb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register CosmosClient
builder.Services.AddSingleton(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    var endpointUrl = configuration["CosmosDb:Account"];
    var primaryKey = configuration["CosmosDb:Key"];
    var connectionString = $"AccountEndpoint={endpointUrl};AccountKey={primaryKey}";

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentNullException(nameof(connectionString), "Cosmos DB connection string is not configured.");
    }

    return new CosmosClient(endpointUrl, primaryKey);
});

// Register EmployeeService
builder.Services.AddScoped<IEmployeeService, EmployeeService>(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    var databaseName = configuration["CosmosDb:DatabaseName"];
    var containerName = configuration["CosmosDb:ContainerName"];

    if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(containerName))
    {
        throw new ArgumentNullException("Cosmos DB database name or container name is not configured.");
    }

    return new EmployeeService(cosmosClient, databaseName, containerName);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Application Lifetime
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopped.Register(() =>
{
    Console.WriteLine("Application is shutting down...");
    // Clean up resources if needed
});

app.Run();
