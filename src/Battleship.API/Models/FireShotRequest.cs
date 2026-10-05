namespace Battleship.API.Models
{
    public record FireShotRequest
    {
        public FireShotRequest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}
