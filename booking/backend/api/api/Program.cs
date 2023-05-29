using service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddCommandLine(args);

builder.Services.AddControllers();

builder.Services
    .AddLogging()
    .AddSingleton<IDatabaseConfiguration>(
        new PostgresDatabaseConfiguration(
            builder.Configuration["ConnectionString"],
            builder.Configuration["EncryptionKey"])
    )
    .AddSingleton<IClientQueryService, PostgresClientQueryService>()
    .AddSingleton<IFirmQueryService, PostgresFirmQueryService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// cors: allow local or carecalendar.xyz
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