using FalveyInsuranceGroup.Db;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. AddNewtonsoftJson is for the Patch method.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

// Add Swagger/Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
