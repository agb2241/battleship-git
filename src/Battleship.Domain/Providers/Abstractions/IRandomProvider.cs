using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Providers.Abstractions
{
    public interface IRandomProvider
    {
        /// <summary>
        /// Returns a random integer within the specified range.
        /// </summary>
        /// <param name="inclusiveLowerBound">
        /// The inclusive lower bound of the random number returned.
        /// </param>
        /// <param name="exclusiveUpperBound">
        /// The exclusive upper bound of the random number returned.
        /// </param>
        /// <returns>
        /// An integer greater than or equal to <paramref name="inclusiveLowerBound"/>
        /// and less than <paramref name="exclusiveUpperBound"/>.
        /// </returns>
        int GetRandomInteger(int inclusiveLowerBound, int exclusiveUpperBound);
    }
}
