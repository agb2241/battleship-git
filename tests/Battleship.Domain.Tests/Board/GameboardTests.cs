using Battleship.Domain.Board;
using Battleship.Domain.Ships;
using Battleship.Domain.Shots;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Tests.Board
{
    public class GameboardTests
    {
        [Theory]
        [InlineData(Direction.Horizontal)]
        [InlineData(Direction.Vertical)]
        public void Gameboard_ShipPlacementOutOfBounds_ReturnsFalse(Direction direction)
        {
            var gameboard = new Gameboard(10);

            var carrierDefinition = new ShipDefinition("Carrier", 5);

            var proposedPlacement = new ProposedShipPlacement(carrierDefinition, new Coordinate(8, 8), direction);

            var result = gameboard.PlaceShip(proposedPlacement);

            Assert.False(result);
            Assert.Empty(gameboard.Ships);
        }

        [Fact]
        public void Gameboard_ShipPlacementOverlap_ReturnsFalse()
        {
            var gameboard = new Gameboard(10);

            var carrierDefinition = new ShipDefinition("Carrier", 5);
            var battleshipDefinition = new ShipDefinition("BattleShip", 4);


            var carrierPlacement = new ProposedShipPlacement(carrierDefinition, new Coordinate(0, 0), Direction.Horizontal);
            var battleshipPlacement = new ProposedShipPlacement(battleshipDefinition, new Coordinate(2, 0), Direction.Vertical);

            var carrierResult = gameboard.PlaceShip(carrierPlacement);
            var battleshipResult = gameboard.PlaceShip(battleshipPlacement);

            Assert.True(carrierResult);
            Assert.False(battleshipResult);
            Assert.Single(gameboard.Ships);
        }

        [Theory]
        [InlineData(Direction.Horizontal, 5, 9)]
        [InlineData(Direction.Vertical, 9, 5)]
        public void Gameboard_ShipPlacementAtBoardEdge_Success(Direction direction, int startX, int startY)
        {
            var gameboard = new Gameboard(10);

            var carrierDefinition = new ShipDefinition("Carrier", 5);

            var proposedPlacement = new ProposedShipPlacement(carrierDefinition, new Coordinate(startX, startY), direction);

            var result = gameboard.PlaceShip(proposedPlacement);

            Assert.True(result);
            Assert.Single(gameboard.Ships);
        }

        [Theory]
        [InlineData(Direction.Horizontal)]
        [InlineData(Direction.Vertical)]
        public void Gameboard_ShipPlacement_Success(Direction direction)
        {
            var gameboard = new Gameboard(10);

            var carrierDefinition = new ShipDefinition("Carrier", 5);

            var proposedPlacement = new ProposedShipPlacement(carrierDefinition, new Coordinate(0,0), direction);

            var result = gameboard.PlaceShip(proposedPlacement);

            Assert.True(result);
            Assert.Single(gameboard.Ships);

            var horizontalExpectedCoordinates = new[]
            {
                new Coordinate(0,0),
                new Coordinate(1,0),
                new Coordinate(2,0),
                new Coordinate(3,0),
                new Coordinate(4,0)
            };

            var verticalExpectedCoordinates = new[]
            {
                new Coordinate(0,0),
                new Coordinate(0,1),
                new Coordinate(0,2),
                new Coordinate(0,3),
                new Coordinate(0,4)
            };

            var expectedCoordinates = direction == Direction.Horizontal ?
                horizontalExpectedCoordinates :
                verticalExpectedCoordinates;

            var actualCoordinates = gameboard.Ships[0].Segments
                .Select(x => x.Coordinate)
                .ToArray();

            Assert.Equal(expectedCoordinates, actualCoordinates);
        }

        [Fact]
        public void Gameboard_FireShotMiss_ReturnsMissAndRecordsShot()
        {
            var gameboard = new Gameboard(10);

            var result = gameboard.FireShot(new Coordinate(5, 5));

            Assert.Equal(ShotOutcome.Miss, result.Outcome);
            Assert.Equal(new Coordinate(5, 5), result.Coordinate);
            Assert.Null(result.SunkShip);
            Assert.Single(gameboard.Shots);
        }

        [Fact]
        public void Gameboard_FireShotHit_ReturnsHitAndRegistersHitOnShip()
        {
            var gameboard = new Gameboard(10);
            var definition = new ShipDefinition("Destroyer", 2);

            var proposedPlacement = new ProposedShipPlacement(definition, new Coordinate(0, 0), Direction.Horizontal);
            gameboard.PlaceShip(proposedPlacement);

            var result = gameboard.FireShot(new Coordinate(0, 0));

            Assert.Equal(ShotOutcome.Hit, result.Outcome);
            Assert.Null(result.SunkShip);
            Assert.True(gameboard.Ships[0].Segments[0].IsHit);
            Assert.Single(gameboard.Shots);
        }

        [Fact]
        public void Gameboard_FireShotSinksShip_ReturnsSunkAndShipDefinition()
        {
            var gameboard = new Gameboard(10);
            var definition = new ShipDefinition("Destroyer", 2);

            var proposedPlacement = new ProposedShipPlacement(definition, new Coordinate(0, 0), Direction.Horizontal);

            gameboard.PlaceShip(proposedPlacement);

            gameboard.FireShot(new Coordinate(0, 0));

            var result = gameboard.FireShot(new Coordinate(1, 0));

            Assert.Equal(ShotOutcome.Sunk, result.Outcome);
            Assert.Equal(definition, result.SunkShip);
            Assert.True(gameboard.Ships[0].IsSunk);
            Assert.Equal(2, gameboard.Shots.Count);
        }

        [Fact]
        public void Gameboard_FireShotDuplicateMiss_ReturnsOriginalResultAndDoesNotRecordAnotherShot()
        {
            var gameboard = new Gameboard(10);
            var coordinate = new Coordinate(5, 5);

            var firstResult = gameboard.FireShot(coordinate);
            var secondResult = gameboard.FireShot(coordinate);

            Assert.Same(firstResult, secondResult);
            Assert.Equal(ShotOutcome.Miss, secondResult.Outcome);
            Assert.Single(gameboard.Shots);
        }

        [Fact]
        public void Gameboard_FireShotDuplicateHit_ReturnsOriginalResultAndDoesNotRecordAnotherShot()
        {
            var gameboard = new Gameboard(10);
            var definition = new ShipDefinition("Destroyer", 2);

            var proposedPlacement = new ProposedShipPlacement(definition, new Coordinate(0, 0), Direction.Horizontal);

            gameboard.PlaceShip(proposedPlacement);

            var firstResult = gameboard.FireShot(new Coordinate(0, 0));
            var secondResult = gameboard.FireShot(new Coordinate(0, 0));

            Assert.Same(firstResult, secondResult);
            Assert.Equal(ShotOutcome.Hit, secondResult.Outcome);
            Assert.Single(gameboard.Shots);
        }

        [Fact]
        public void Gameboard_FireShotOutOfBounds_ThrowsArgumentOutOfRangeException()
        {
            var gameboard = new Gameboard(10);

            Assert.Throws<ArgumentOutOfRangeException>(() => gameboard.FireShot(new Coordinate(50, 50)));

            Assert.Empty(gameboard.Shots);
        }
    }
}
