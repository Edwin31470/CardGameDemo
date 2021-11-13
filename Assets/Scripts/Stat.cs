using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Stat
    {
        private int _value;

        private int _lowerBound;
        private int _upperBound;

        public Stat(int lowerBound, int upperBound, int defaultValue)
        {
            _lowerBound = lowerBound;
            _upperBound = upperBound;
            Set(defaultValue);
        }

        public int Get()
        {
            return _value;
        }

        public void Set(int value)
        {
            value = Math.Max(value, _lowerBound);
            value = Math.Min(value, _upperBound);
            _value = value;
        }

        public void Add(int value)
        {
            Set(_value + value);
        }

        public void Remove(int value)
        {
            Add(-value);
        }
    }
}
