using System.Data;
using api;
using model.interfaces;
using Npgsql;
using service.postgres;

Console.WriteLine("Starting");

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args);
builder.Services.AddControllers();

builder.Services
    .AddEnvironmentSpecificServices(builder.Environment)
    .AddLogging()
    .AddScoped<IDbConnection>(p =>
    {
        var provider = p.GetRequiredService<IConfigManager>();
        
        return new NpgsqlConnection(provider.ConnectionString);
    })
    .AddScoped<IFirmService, PostgresFirmService>()
    .AddScoped<IPersonService, PostgresPersonService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .WithOrigins(
        "http://localhost:5000",
        "https://carecalendar.xyz"
    )
    .WithMethods("GET", "POST", "PUT", "DELETE")
    .WithHeaders("Content-Type")
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();