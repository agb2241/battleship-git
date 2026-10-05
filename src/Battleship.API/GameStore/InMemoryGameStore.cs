using Battleship.API.GameStore.Abstractions;
using Battleship.Domain;
using System.Collections.Concurrent;

namespace Battleship.API.GameStore
{
    public class InMemoryGameStore : IGameStore
    {
        private readonly ConcurrentDictionary<Guid, Game> _games = new();

        public void Add(Game game)
        {
            if(_games.TryAdd(game.Id, game) == false)
            {
                throw new InvalidOperationException($"Game {game.Id} already exists.");
            }
        }

        public Game? Get(Guid gameId)
        {
            _games.TryGetValue(gameId, out var game);

            return game;
        }
    }
}
