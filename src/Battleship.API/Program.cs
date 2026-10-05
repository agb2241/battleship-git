using Battleship.API.GameStore;
using Battleship.API.GameStore.Abstractions;
using Battleship.Data;
using Battleship.Data.Services;
using Battleship.Domain.Factories;
using Battleship.Domain.Providers;
using Battleship.Domain.Providers.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BattleshipDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Battleship"))
);

builder.Services
    .AddSingleton<GameFactory>()
    .AddSingleton<IGameStore, InMemoryGameStore>()
    .AddSingleton<IRandomProvider, RandomProvider>();

builder.Services
    .AddScoped<CompletedGameService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Battleship API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
