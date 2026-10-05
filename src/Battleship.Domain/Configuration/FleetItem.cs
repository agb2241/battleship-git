using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Configuration
{
    public class FleetItem
    {
        public FleetItem(ShipDefinition definition, int quantity)
        {
            if(quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity of a ship type being placed into the fleet must be greater than 0");
            }

            Definition = definition;
            Quantity = quantity;
        }

        public ShipDefinition Definition { get; }
        public int Quantity { get; }
    }
}
