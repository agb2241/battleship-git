using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Board
{
    public class ProposedShipPlacement
    {
        public ProposedShipPlacement(ShipDefinition shipDefinition, Coordinate startingCoordinate, Direction direction)
        {
            Definition = shipDefinition;
            StartCoordinate = startingCoordinate;
            Direction = direction;
        }

        public ShipDefinition Definition { get; }
        public Coordinate StartCoordinate { get; }
        public Direction Direction { get; }

        public IEnumerable<Coordinate> GetCoordinates()
        {
            var coordinates = new List<Coordinate>() { StartCoordinate };

            for (int i = 1; i < Definition.Length; i++)
            {
                var x = Direction == Direction.Horizontal ? StartCoordinate.X + i : StartCoordinate.X;
                var y = Direction == Direction.Vertical ? StartCoordinate.Y + i : StartCoordinate.Y;

                coordinates.Add(new Coordinate(x, y));
            }

            return coordinates;
        }
    }
}
