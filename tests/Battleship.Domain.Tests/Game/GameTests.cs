using Battleship.Domain.Board;
using Battleship.Domain.Configuration;
using Battleship.Domain.Ships;
using Battleship.Domain.Shots;
using Battleship.Domain.Tests.TestProviders;

namespace Battleship.Domain.Tests.Game
{
    public class GameTests
    {
        [Fact]
        public void Game_Creation_PlacesConfiguredFleetAtDeterministicLocations()
        {
            var fleet = new[]
            {
                new FleetItem(new ShipDefinition("Carrier", 5), 1),
                new FleetItem(new ShipDefinition("Battleship", 4), 1),
                new FleetItem(new ShipDefinition("Destroyer", 2), 1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1, // Carrier:     (0,0), Horizontal
                0, 2, 1, // Battleship:  (0,2), Horizontal
                0, 4, 1  // Destroyer:   (0,4), Horizontal
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            Assert.Equal(3, game.Gameboard.Ships.Count);

            Assert.Equal(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(2, 0),
                new Coordinate(3, 0),
                new Coordinate(4, 0)
            },
            game.Gameboard.Ships[0].Segments
                .Select(x => x.Coordinate)
                .ToArray());

            Assert.Equal(new[]
            {
                new Coordinate(0, 2),
                new Coordinate(1, 2),
                new Coordinate(2, 2),
                new Coordinate(3, 2)
            },
            game.Gameboard.Ships[1].Segments
                .Select(x => x.Coordinate)
                .ToArray());

            Assert.Equal(new[]
            {
                new Coordinate(0, 4),
                new Coordinate(1, 4)
            },
            game.Gameboard.Ships[2].Segments
                .Select(x => x.Coordinate)
                .ToArray());
        }

        [Fact]
        public void Game_ShipPlacementFails_RetriesUntilSuccessful()
        {
            var fleet = new[]
            {
                new FleetItem(new ShipDefinition("Carrier", 5), 1),
                new FleetItem(new ShipDefinition("Battleship", 4), 1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1, // Carrier: succeeds horizontally at (0,0)
                0, 0, 1, // Battleship: fails - overlaps Carrier
                0, 2, 1  // Battleship: succeeds
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            Assert.Equal(2, game.Gameboard.Ships.Count);

            var battleshipCoordinates = game.Gameboard.Ships[1].Segments
                .Select(x => x.Coordinate)
                .ToArray();

            var expectedCoordinates = new[]
            {
                new Coordinate(0, 2),
                new Coordinate(1, 2),
                new Coordinate(2, 2),
                new Coordinate(3, 2)
            };

            Assert.Equal(expectedCoordinates, battleshipCoordinates);
        }

        [Fact]
        public void Game_FireShot_ReturnsCorrectShotSummary()
        {
            var fleet = new[]
            {
                new FleetItem(new ShipDefinition("Destroyer", 2), 1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1 // Destroyer: (0,0) - (1,0), Horizontal
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            var result = game.FireShot(new Coordinate(0, 0));

            Assert.Equal(ShotOutcome.Hit, result.ShotResult.Outcome);
            Assert.Equal(new Coordinate(0, 0), result.ShotResult.Coordinate);
            Assert.Equal(1, result.ShotsFired);
            Assert.Equal(1, result.ShipsRemaining);
            Assert.False(result.IsWon);
        }

        [Fact]
        public void Game_FireShotWinningShot_ReturnsWonSummary()
        {
            var fleet = new[]
            {
                new FleetItem(new ShipDefinition("Destroyer", 2), 1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1 // Destroyer: (0,0) - (1,0), Horizontal
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            game.FireShot(new Coordinate(0, 0));

            var result = game.FireShot(new Coordinate(1, 0));

            Assert.Equal(ShotOutcome.Sunk, result.ShotResult.Outcome);
            Assert.Equal("Destroyer", result.ShotResult.SunkShip?.Name);
            Assert.Equal(2, result.ShotsFired);
            Assert.Equal(0, result.ShipsRemaining);
            Assert.True(result.IsWon);
            Assert.True(game.IsWon);
        }

        [Fact]
        public void Game_FireShotAfterGameWon_ThrowsInvalidOperationException()
        {
            var fleet = new[]
            {
                new FleetItem(new ShipDefinition("Destroyer", 2), 1)
            };

            var fleetConfiguration = new FleetConfiguration(fleet);
            var gameConfiguration = new GameConfiguration(10, fleetConfiguration);

            var randomProvider = new TestRandomProvider(new[]
            {
                0, 0, 1 // Destroyer: (0,0) - (1,0), Horizontal
            });

            var game = new Domain.Game(randomProvider, gameConfiguration);

            game.FireShot(new Coordinate(0, 0));
            game.FireShot(new Coordinate(1, 0));

            Assert.True(game.IsWon);

            Assert.Throws<InvalidOperationException>(() => game.FireShot(new Coordinate(5, 5)));

            Assert.Equal(2, game.Gameboard.Shots.Count);
        }

    }
}
