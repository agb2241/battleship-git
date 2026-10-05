namespace Battleship.API.Models
{
    public record CreateGameResponse
    {
        public CreateGameResponse(Guid gameId, int boardSize, int shipCount)
        {
            GameId = gameId;
            BoardSize = boardSize;
            ShipCount = shipCount;
        }

        public Guid GameId { get; }
        public int BoardSize { get; }
        public int ShipCount { get; set; }
    }
}
