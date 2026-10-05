namespace Battleship.API.Models
{
    public record ShotResponse
    {
        public ShotResponse(int x, int y, string outcome, string? shipSunk)
        {
            X = x;
            Y = y;
            Outcome = outcome;
            ShipSunk = shipSunk;
        }

        public int X { get; }
        public int Y { get; }
        public string Outcome { get; }
        public string? ShipSunk { get; }
    }
}
