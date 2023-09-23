using api;
using model.interfaces;
using service;
using service.postgres;
using IConfiguration = model.interfaces.IConfiguration;

Console.WriteLine("Starting");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddCommandLine(args);

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Using local configuration");
    builder.Services.AddSingleton<IConfiguration, LocalConfiguration>();
    builder.Services.AddSingleton<IEncryption, NoEncryption>();
}
else
{
    builder.Services.AddSingleton<IConfiguration, AwsConfiguration>();
    builder.Services.AddSingleton<IEncryption, AwsEncryption>();
}

builder.Services
    .AddLogging()
    .AddSingleton<IFirmService, PostgresFirmService>()
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
