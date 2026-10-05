using Battleship.Domain.Providers.Abstractions;

namespace Battleship.Domain.Providers
{
    public class RandomProvider : IRandomProvider
    {
        public int GetRandomInteger(int lowerBound, int upperBound)
        {
            var random = new Random();

            return random.Next(lowerBound, upperBound);
        }
    }
}
