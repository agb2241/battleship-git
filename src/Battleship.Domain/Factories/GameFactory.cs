using Battleship.Domain.Configuration;
using Battleship.Domain.Providers.Abstractions;
using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Battleship.Domain.Factories
{
    public class GameFactory
    {
        private readonly IRandomProvider _randomProvider;
        private const int StandardBoardSize = 10;
        private static readonly ImmutableArray<FleetItem> StandardFleet =
            ImmutableArray.Create(
                new FleetItem(new ShipDefinition("Aircraft Carrier", 5), 1),
                new FleetItem(new ShipDefinition("Battleship", 4), 1),
                new FleetItem(new ShipDefinition("Cruiser", 3), 1),
                new FleetItem(new ShipDefinition("Submarine", 3), 1),
                new FleetItem(new ShipDefinition("Destroyer", 2), 1));

        public GameFactory(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider;
        }

        public Game CreateGame()
        {
            var fleetConfig = new FleetConfiguration(StandardFleet);
            var gameConfig = new GameConfiguration(StandardBoardSize, fleetConfig);

            return new Game(_randomProvider, gameConfig);
        }
    }
}
