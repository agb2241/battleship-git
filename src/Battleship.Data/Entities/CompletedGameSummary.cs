using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Data.Entities
{
    public class CompletedGameSummary
    {
        public Guid GameId { get; set; }
        public int BoardSize { get; set; }
        public int ShipCount { get; set; }
        public int TotalShots { get; set; }
        public DateTime CompletedAtUtc { get; set; }
    }
}
