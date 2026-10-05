namespace Battleship.API.Models
{
    public record CreateGameResponse
    {
        public CreateGameResponse(Guid gameId, int boardSize)
        {
            GameId = gameId;
            BoardSize = boardSize;
        }

        public Guid GameId { get; }
        public int BoardSize { get; }
    }
}
