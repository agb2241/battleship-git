using Battleship.Domain.Board;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Battleship.Domain.Ships
{
    public class Ship
    {
        public Ship(ShipDefinition definition, IEnumerable<ShipSegment> segments)
        {
            Definition = definition;
            Segments = segments.ToImmutableList();
        }

        public ShipDefinition Definition { get;}
        public IReadOnlyList<ShipSegment> Segments { get; }
        
        public bool IsSunk => Segments.All(x => x.IsHit == true);


        public void RegisterHit(Coordinate coord)
        {
            var segment = Segments.FirstOrDefault(x => x.Coordinate == coord);

            if(segment == null)
            {
                throw new ArgumentException($"The coordinate {coord.ToString()} does not belong to this ship.");
            }

            segment.RegisterHit();
        }
    }
}
