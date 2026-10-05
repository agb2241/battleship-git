using Battleship.Data;
using Battleship.Data.Services;
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
    .AddSingleton<IRandomProvider, RandomProvider>()
    .AddScoped<CompletedGameService>(); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
