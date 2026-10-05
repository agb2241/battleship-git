namespace Battleship.API.Models
{
    public record GameStateResponse
    {
        public GameStateResponse(Guid gameId, int boardSize, int shotsFired, int shipsRemaining, bool isWon, IReadOnlyList<ShotResponse> shots)
        {
            GameId = gameId;
            BoardSize = boardSize;
            ShotsFired = shotsFired;
            IsWon = isWon;
            Shots = shots;
        }

        public Guid GameId { get; }
        public int BoardSize { get; }
        public int ShotsFired { get; }
        public int ShipsRemaining { get; }
        public bool IsWon { get; }
        public IReadOnlyList<ShotResponse> Shots { get; set; }
    }
}
