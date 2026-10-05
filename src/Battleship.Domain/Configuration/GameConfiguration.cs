using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Configuration
{
    public class GameConfiguration
    {
        public GameConfiguration(int boardSize, FleetConfiguration fleetConfiguration)
        {
            if(boardSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(boardSize), "Board size must be greater than 0.");
            }

            if(fleetConfiguration == null)
            {
                ArgumentNullException.ThrowIfNull(fleetConfiguration);
            }

            if (fleetConfiguration.Fleet.Any(x => x.Definition.Length > boardSize)) 
            {
                throw new ArgumentException("You cannot place a ship on a board whose size is less than the length of the ship.");
            }

            if(boardSize * boardSize < fleetConfiguration.Fleet.Sum(x => x.Definition.Length * x.Quantity))
            {
                throw new ArgumentException("The suggested board size cannot accommodate all suggested ships.");
            }

            BoardSize = boardSize;
            FleetConfiguration = fleetConfiguration;
        }

        public int BoardSize { get; }
        public FleetConfiguration FleetConfiguration { get;}


    }
}
