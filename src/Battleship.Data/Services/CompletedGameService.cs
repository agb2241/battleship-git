using Battleship.Data.Entities;
using Battleship.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Data.Services
{
    public class CompletedGameService
    {
        private readonly BattleshipDbContext _dbContext;

        public CompletedGameService(BattleshipDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveCompletedGameAsync(Game game)
        {
            if (game.IsWon == false)
            {
                throw new InvalidOperationException("A game cannot be persisted until it has been won.");
            }

            var summary = new CompletedGameSummary
            {
                GameId = game.Id,
                BoardSize = game.GameConfiguration.BoardSize,
                ShipCount = game.Gameboard.Ships.Count,
                TotalShots = game.Gameboard.Shots.Count,
                CompletedAtUtc = DateTime.UtcNow
            };

            _dbContext.CompletedGameSummaries.Add(summary);

            await _dbContext.SaveChangesAsync();
        }
    }
}
