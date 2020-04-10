using Velo.Metrics.Counters;

namespace Velo.Metrics.Provider
{
    public interface IMetricsProvider
    {
        ICounter[] Counters { get; }
        
        ICounter GetCounter(string name);
    }
}