using Battleship.Domain;

namespace Battleship.API.GameStore.Abstractions
{
    public interface IGameStore
    {
        void Add(Game game);
        Game? Get(Guid gameId);

    }
}
