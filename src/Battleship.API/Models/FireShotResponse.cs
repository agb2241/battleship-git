namespace Battleship.API.Models
{
    public record FireShotResponse
    {
        public FireShotResponse(string outcome, string? sunkShip, int shotsFired, int shipsRemaining, bool isWon)
        {
            Outcome = outcome;
            SunkShip = sunkShip;
            ShotsFired = shotsFired;
            ShipsRemaining = shipsRemaining;
            IsWon = isWon;
        }

        public string Outcome { get; }
        public string? SunkShip { get; }
        public int ShotsFired { get; }
        public int ShipsRemaining { get; }
        public bool IsWon { get; }
    }
}
