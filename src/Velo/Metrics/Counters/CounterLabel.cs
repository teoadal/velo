using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Velo.Utils;

namespace Velo.Metrics.Counters
{
    [DebuggerDisplay("{Value} {Name}")]
    public class CounterLabel : ICounterLabel
    {
        public string Name { get; }

        public double Value
        {
            get
            {
                var value = Volatile.Read(ref _value);
                return BitConverter.Int64BitsToDouble(value);
            }
        }

        private long _value;

        public CounterLabel(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw Error.Null(nameof(name));

            Name = name;
        }

        public double Increment(double value = 1d)
        {
            if (value <= 0d) throw Error.InvalidOperation($"Increment value must be greater 0");
            
            while (true)
            {
                var initialRaw = _value;
                var computedValue = BitConverter.Int64BitsToDouble(initialRaw) + value;

                if (TryChange(computedValue, initialRaw))
                {
                    return computedValue;
                }
            }
        }

        public double IncrementTo(double targetValue)
        {
            while (true)
            {
                var initialRaw = _value;
                var initialValue = BitConverter.Int64BitsToDouble(initialRaw);

                if (initialValue >= targetValue)
                {
                    return initialValue;
                }

                if (TryChange(targetValue, initialRaw))
                {
                    return targetValue;
                }
            }
        }

        public double Flush()
        {
            while (true)
            {
                var initialRaw = _value;
                var initialValue = BitConverter.Int64BitsToDouble(initialRaw);

                if (TryChange(0d, initialRaw))
                {
                    return initialValue;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryChange(double target, long initialRaw)
        {
            return initialRaw == Interlocked.CompareExchange(
                ref _value,
                BitConverter.DoubleToInt64Bits(target),
                initialRaw);
        }
    }
}