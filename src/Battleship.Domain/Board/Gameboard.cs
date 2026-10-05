using Battleship.Domain.Ships;
using Battleship.Domain.Shots;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Battleship.Domain.Board
{
    public class Gameboard
    {
        public Gameboard(int size)
        {
            Size = size;
        }

        public int Size { get; }

        private readonly List<Ship> _ships = new();
        public IReadOnlyList<Ship> Ships => _ships;

        private readonly List<ShotResult> _shots = new();
        public IReadOnlyList<ShotResult> Shots => _shots;

        public bool PlaceShip(ProposedShipPlacement shipPlacement)
        {
            var shipSegments = new List<ShipSegment>();

            foreach (var coordinate in shipPlacement.GetCoordinates())
            {
                // Bounds check
                if (IsValidCoordinate(coordinate) == false)
                {
                    return false;
                }

                shipSegments.Add(new ShipSegment(coordinate));
            }
            // Add our ship
            _ships.Add(new Ship(shipPlacement.Definition, shipSegments));

            return true;
        }

        public ShotResult FireShot(Coordinate coordinate)
        {

            if (IsCoordinateInBounds(coordinate) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate), "The shot was out of bounds.");
            }

            ShotResult result;

            var alreadyFiredUpon = _shots.FirstOrDefault(x => x.Coordinate == coordinate);
            if (alreadyFiredUpon != null)
            {
                return alreadyFiredUpon;
            }

            var ship = Ships.FirstOrDefault(x => x.Segments.Any(segment => segment.Coordinate == coordinate));
            if (ship == null)
            {
                result = new ShotResult(coordinate, ShotOutcome.Miss);
            }
            else 
            {
                ship.RegisterHit(coordinate);

                var isSunk = ship.IsSunk;

                ShotOutcome outcome = isSunk == true ? ShotOutcome.Sunk : ShotOutcome.Hit;
                ShipDefinition? shipDefinition = isSunk == true ? ship.Definition : null;
                result = new ShotResult(coordinate, outcome, shipDefinition);
            }

            _shots.Add(result);
            return result;
        }

        private bool IsValidCoordinate(Coordinate coordinate)
        {
            if (!IsCoordinateInBounds(coordinate) || !IsFreeOfOverlap(coordinate))
            {
                return false;
            }

            return true;
        }

        private bool IsFreeOfOverlap(Coordinate coordinate)
        {
            if (Ships.Any(ship => ship.Segments.Any(segment => segment.Coordinate == coordinate)))
            {
                return false;
            }

            return true;
        }

        private bool IsCoordinateInBounds(Coordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < Size &&
                coordinate.Y >= 0 && coordinate.Y < Size;
        }
    }
}


// Responsibilities / ideas:
// - Possible refactor: Add a Rules engine and rules that can be applied to the gameboard. 