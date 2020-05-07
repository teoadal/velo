using System.Linq;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Metrics.Counters
{
    internal sealed class Counter<TSender> : ICounter<TSender>
    {
        public string Description { get; }

        public ICounterLabel[] Labels => _labels;

        public string Name { get; }

        private readonly ICounterLabel[] _labels;

        public Counter(string name, string description, ICounterLabel[]? labels)
        {
            if (string.IsNullOrWhiteSpace(name)) throw Error.Null(nameof(name));
            if (labels == null) throw Error.Null(nameof(labels));

            if (labels.Select(label => label.Name).Distinct().Count() != labels.Length)
            {
                throw Error.InvalidOperation($"All label names must be unique");
            }

            Name = name;
            Description = description;

            _labels = labels;
        }

        public double Increment(string label, double value = 1d)
        {
            var counterLabel = GetLabel(label);
            return counterLabel.Increment(value);
        }

        public double IncrementTo(string label, double targetValue)
        {
            var counterLabel = GetLabel(label);
            return counterLabel.IncrementTo(targetValue);
        }

        public double this[string label] => GetLabel(label).Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ICounterLabel GetLabel(string name)
        {
            foreach (var label in _labels)
            {
                if (label.Name.Equals(name))
                {
                    return label;
                }
            }

            throw Error.NotFound($"Label with name {name} not registered");
        }
    }
}