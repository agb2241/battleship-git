using Battleship.Domain.Board;
using Battleship.Domain.Configuration;
using Battleship.Domain.Providers.Abstractions;
using Battleship.Domain.Ships;
using Battleship.Domain.Shots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain
{
    public class Game
    {
        private readonly IRandomProvider _randomProvider;
        private const int MaxPlacementAttempts = 100;

        public Game(IRandomProvider randomProvider, GameConfiguration gameConfiguration)
        {
            _randomProvider = randomProvider;
            GameConfiguration = gameConfiguration;

            // Create the game board
            Gameboard = new Gameboard(GameConfiguration.BoardSize);

            PlaceFleet();
        }

        public Gameboard Gameboard { get; }
        public GameConfiguration GameConfiguration { get; }

        public bool IsWon => Gameboard.Ships.All(x => x.IsSunk);
        public int ShipsRemaining => Gameboard.Ships.Count(x => x.IsSunk == false);



        private void PlaceFleet()
        {
            foreach(var ship in GameConfiguration.FleetConfiguration.Fleet)
            {
                for (int i = 0; i < ship.Quantity; i++)
                {
                    var shipPlaced = false;
                    var placementAttempt = 0;

                    while (shipPlaced == false && placementAttempt < MaxPlacementAttempts)
                    {
                        var x = _randomProvider.GetRandomInteger(0, GameConfiguration.BoardSize);
                        var y = _randomProvider.GetRandomInteger(0, GameConfiguration.BoardSize);
                        var direction = _randomProvider.GetRandomInteger(0, 2) == 0 ?
                            Direction.Vertical :
                            Direction.Horizontal;

                        var proposedShipPlacement = new ProposedShipPlacement(ship.Definition, new Coordinate(x, y), direction);

                        shipPlaced = Gameboard.PlaceShip(proposedShipPlacement);
                        placementAttempt++;
                    }

                    if (shipPlaced == false)
                    {
                        throw new InvalidOperationException($"Unable to place {ship.Definition.Name} after {MaxPlacementAttempts} attempts.");
                    }
                }
            }


        }

        public ShotSummary FireShot(Coordinate coordinate)
        {
            if(IsWon == true)
            {
                throw new InvalidOperationException("The game is already won. You cannot continue firing shots.");
            }

            var shotResult = Gameboard.FireShot(coordinate);

            return new ShotSummary(shotResult, Gameboard.Shots.Count, ShipsRemaining, IsWon);
        }
    }
}