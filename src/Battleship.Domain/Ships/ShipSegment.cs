using Battleship.Domain.Board;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Ships
{
    public class ShipSegment
    {
        public ShipSegment(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public Coordinate Coordinate { get; }
        public bool IsHit { get; private set; }

        public void RegisterHit()
        {
            if(IsHit == true)
            {
                throw new InvalidOperationException("This ship segment was already hit. You cannot hit it again.");
            }

            IsHit = true;
        }
    }
}
