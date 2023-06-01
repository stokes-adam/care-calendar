using model.interfaces;
using service;
using service.postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddCommandLine(args);
var connectionString = builder.Configuration["ConnectionString"] ?? throw new Exception("Missing connection string");
var encryptionKey = builder.Configuration["EncryptionKey"] ?? throw new Exception("Missing encryption key");
var config = new DbConfiguration(connectionString, encryptionKey);

builder.Services.AddControllers();

builder.Services
    .AddLogging()
    .AddSingleton<IDbConfiguration>(config)
    .AddSingleton<Encryption>()
    .AddSingleton<IFirmQueryService, PostgresFirmQueryService>()
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
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();