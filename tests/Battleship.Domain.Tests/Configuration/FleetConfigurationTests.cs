using Battleship.Domain.Configuration;
using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Tests.Configuration
{
    public class FleetConfigurationTests
    {
        [Fact]
        public void FleetConfiguration_EmptyCollection_ThrowsArgumentException()
        {
            List<FleetItem> items = new List<FleetItem>();

            Assert.Throws<ArgumentException>(() => new FleetConfiguration(items));
        }

        [Fact]
        public void FleetConfiguration_DuplicateEntries_ThrowsArgumentException()
        {
            List<FleetItem> items = new List<FleetItem>()
            {
                new FleetItem(new ShipDefinition("Carrier", 5), 1),
                new FleetItem(new ShipDefinition("Carrier", 4), 1),
                new FleetItem(new ShipDefinition("Battleship", 4), 1),
            };

            Assert.Throws<ArgumentException>(() => new FleetConfiguration(items));
        }

        [Fact]
        public void FleetConfiguration_Creation_Success()
        {
            List<FleetItem> items = new List<FleetItem>()
            {
                new FleetItem(new ShipDefinition("Carrier", 5), 3),
                new FleetItem(new ShipDefinition("Submarine", 3), 2),
                new FleetItem(new ShipDefinition("Battleship", 4), 1),
            };

            var fleetConfig = new FleetConfiguration(items);

            Assert.Equal(items, fleetConfig.Fleet);
        }
    }
}
