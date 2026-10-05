namespace Battleship.API.Models
{
    public record CompletedGameSummaryResponse
    {
        public CompletedGameSummaryResponse(Guid gameId, int boardSize, int shipCount, int totalShots, DateTime completedAtUtc)
        {
            GameId = gameId;
            BoardSize = boardSize;
            ShipCount = shipCount;
            TotalShots = totalShots;
            CompletedAtUtc = completedAtUtc;
        }

        public Guid GameId { get; }
        public int BoardSize { get; }
        public int ShipCount { get; }
        public int TotalShots { get; }
        public DateTime CompletedAtUtc { get; }
    }
}
