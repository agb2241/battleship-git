using Battleship.Data.Services;
using Battleship.Domain.Board;
using Battleship.Domain.Configuration;
using Battleship.Domain.Ships;
using Battleship.Domain.Tests.TestProviders;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Data.Tests
{
    public class CompletedGameServiceTests
    {
        [Fact]
        public async Task SaveCompletedGameAsync_CompletedGame_PersistsCorrectSummary()
        {
            // Arrange
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<BattleshipDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var dbContext = new BattleshipDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();

            var fleet = new[]
            {
                new FleetItem(
                    new ShipDefinition("Destroyer", 2),
                    1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            // Miss
            game.FireShot(new Coordinate(5, 5));

            // Hit
            game.FireShot(new Coordinate(0, 0));

            // Hit + sink + win
            game.FireShot(new Coordinate(1, 0));

            var service = new CompletedGameService(dbContext);

            // Act
            await service.SaveCompletedGameAsync(game);

            // Assert
            var summary = await dbContext.CompletedGameSummaries.SingleAsync();

            Assert.Equal(game.Id, summary.GameId);
            Assert.Equal(10, summary.BoardSize);
            Assert.Equal(1, summary.ShipCount);
            Assert.Equal(3, summary.TotalShots);
            Assert.True(summary.CompletedAtUtc > DateTime.MinValue);
        }
    }
}
