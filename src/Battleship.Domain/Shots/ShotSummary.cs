using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Shots
{
    public class ShotSummary
    {
        public ShotSummary(ShotResult shotResult, int shotsFired, int shipsRemaining, bool isWon)
        {
            ShotResult = shotResult;
            ShotsFired = shotsFired;
            ShipsRemaining = shipsRemaining;
            IsWon = isWon;
        }

        public ShotResult ShotResult { get; }
        public int ShotsFired { get; }
        public int ShipsRemaining { get; }
        public bool IsWon { get; }
    }
}
