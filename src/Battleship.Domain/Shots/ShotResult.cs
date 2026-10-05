using Battleship.Domain.Board;
using Battleship.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Battleship.Domain.Shots
{
    public class ShotResult
    {
        public ShotResult(Coordinate coordinate, ShotOutcome outcome, ShipDefinition? definition = null)
        {
            Coordinate = coordinate;
            Outcome = outcome;
            SunkShip = definition;
        }

        public Coordinate Coordinate { get; }
        public ShotOutcome Outcome { get; }
        public ShipDefinition? SunkShip { get; set; }
    }
}
