using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Board
{
    public readonly record struct Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString() 
        {
            return $"{X},{Y}";
        }

    }
}
