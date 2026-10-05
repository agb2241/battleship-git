using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Battleship.Domain.Ships
{
    public readonly record struct ShipDefinition
    {
        public ShipDefinition(string name, int length)
        {
            Name = name;
            Length = length;
        }

        public string Name { get; }
        public int Length { get; }
    }
}
