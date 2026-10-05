using Battleship.Domain.Configuration;
using Battleship.Domain.Ships;

namespace Battleship.Domain.Tests.Configuration
{
    public class FleetItemTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void FleetItem_QuantityIsZeroOrNegative_ThrowsArgumentOutOfRangeException(int quantity)
        {
            var definition = new ShipDefinition("Destroyer", 2);

            Assert.Throws<ArgumentOutOfRangeException>(() => new FleetItem(definition, quantity));
        }

        [Fact]
        public void FleetItem_Creation_Success()
        {
            var definition = new ShipDefinition("Destroyer", 2);

            var fleetItem = new FleetItem(definition, 1);

            Assert.Equal(definition, fleetItem.Definition);
            Assert.Equal(1, fleetItem.Quantity);
        }
    }
}
