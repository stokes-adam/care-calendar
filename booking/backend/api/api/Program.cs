using Amazon.KeyManagementService;
using api;
using model.interfaces;
using service;
using service.postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddCommandLine(args)
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services
    .AddLogging()
    .AddAwsServices()
    .AddPostgresServices()
    .AddSingleton<AwsEncryption>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
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
