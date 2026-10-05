using Battleship.Domain.Providers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship.Domain.Tests.TestProviders
{
    public class TestRandomProvider : IRandomProvider
    {
        private readonly Queue<int> _values;

        public TestRandomProvider(IEnumerable<int> values)
        {
            _values = new Queue<int>(values);
        }

        public int GetRandomInteger(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("No more predetermined random values are available.");
            }

            return _values.Dequeue();
        }
    }
}
