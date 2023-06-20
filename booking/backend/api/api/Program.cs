using api;
using model.interfaces;
using service;
using service.postgres;

Console.WriteLine("Starting");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddCommandLine(args);
var appSettings = new AppSettings();
var connectionString = appSettings.GetConnectionString();
var encryptionKey = appSettings.GetEncryptionKey();
var config = new DbConfiguration(connectionString, encryptionKey);

builder.Services.AddControllers();

builder.Services
    .AddLogging()
    .AddSingleton<IDbConfiguration>(config)
    .AddSingleton<Encryption>()
    .AddSingleton<IFirmQueryService, PostgresFirmQueryService>()
    .AddSingleton<IFirmCommandService, PostgresFirmCommandService>()
    .AddSingleton<IPersonQueryService, PostgresPersonQueryService>()
    .AddSingleton<IPersonCommandService, PostgresPersonCommandService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .WithOrigins(
        "http://localhost:3000",
        "https://carecalendar.xyz"
    )
    .WithMethods("GET", "POST", "PUT", "DELETE")
    .WithHeaders("Content-Type")
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
