using Velo.Metrics.Counters;
using Velo.Utils;

namespace Velo.Metrics.Provider
{
    internal sealed class MetricsProvider : IMetricsProvider
    {
        public ICounter[] Counters { get; }

        public MetricsProvider(ICounter[] counters)
        {
            Counters = counters;
        }

        public ICounter GetCounter(string name)
        {
            foreach (var counter in Counters)
            {
                if (counter.Name == name) return counter;
            }

            throw Error.NotFound("Counter with name '{name}' isn't registered");
        }
    }
}