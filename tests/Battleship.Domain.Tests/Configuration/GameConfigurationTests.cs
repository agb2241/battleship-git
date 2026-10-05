using Battleship.Domain.Configuration;
using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Tests.Configuration
{
    public class GameConfigurationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void GameConfiguration_BoardSizeIsZeroOrNegative_ThrowsArgumentOutOfRangeException(int boardSize)
        {
            var fleetConfiguration = CreateValidFleetConfiguration();

            Assert.Throws<ArgumentOutOfRangeException>(() => new GameConfiguration(boardSize, fleetConfiguration));
        }

        [Fact]
        public void GameConfiguration_ShipLongerThanBoard_ThrowsArgumentException()
        {
            var fleet = new List<FleetItem>
        {
            new(new ShipDefinition("Carrier", 5), 1)
        };

            var fleetConfiguration = new FleetConfiguration(fleet);

            Assert.Throws<ArgumentException>(() => new GameConfiguration(4, fleetConfiguration));
        }

        [Fact]
        public void GameConfiguration_FleetRequiresMoreSpacesThanBoard_ThrowsArgumentException()
        {
            var fleet = new List<FleetItem>
        {
            new(new ShipDefinition("Destroyer", 2), 5)
        };

            var fleetConfiguration = new FleetConfiguration(fleet);

            Assert.Throws<ArgumentException>(() => new GameConfiguration(3, fleetConfiguration));
        }

        [Fact]
        public void GameConfiguration_ValidConfiguration_CreatesSuccessfully()
        {
            var fleetConfiguration = CreateValidFleetConfiguration();

            var configuration = new GameConfiguration(10, fleetConfiguration);

            Assert.Equal(10, configuration.BoardSize);
            Assert.Equal(fleetConfiguration, configuration.FleetConfiguration);
        }

        private static FleetConfiguration CreateValidFleetConfiguration()
        {
            var fleet = new List<FleetItem>
            {
                new(new ShipDefinition("Carrier", 5), 1),
                new(new ShipDefinition("Battleship", 4), 1),
                new(new ShipDefinition("Submarine", 3), 1),
                new(new ShipDefinition("Destroyer", 2), 1)
            };

            return new FleetConfiguration(fleet);
        }
    }
}
