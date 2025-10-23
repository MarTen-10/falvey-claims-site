using FalveyInsuranceGroup.Db;
using backend.services;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Configuration;
using FalveyInsuranceGroup.Backend.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. AddNewtonsoftJson is for the Patch method.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddScoped<InputService>();
// Add Swagger/Swashbuckle and inject certain services for certain controllers
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration - Allow your frontend to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",      // React default
                "http://localhost:5173",      // Vite default
                "http://localhost:8080",      // Vue default
                "http://127.0.0.1:5500",      // Live Server
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173",
                "null"                        // For file:// protocol (local HTML files)
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection failed.");
}

builder.Services.AddDbContext<FalveyInsuranceGroupContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FalveyInsuranceGroup API V1");
        c.RoutePrefix = "swagger"; // optional, default is "swagger"
    });
}

// Test database connection
app.MapGet("/api/test/db", async (FalveyInsuranceGroupContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new
        {
            connected = canConnect,
            message = canConnect ? "Database connection successful" : "Cannot connect to database"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500,
            title: "Database Connection Failed"
        );
    }
});

app.UseHttpsRedirection();

// IMPORTANT: Use CORS before Authorization and MapControllers
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();
app.Run();