using System;
using Velo.Metrics.Counters;
using Velo.Utils;

namespace Velo.Metrics.Provider
{
    internal sealed class NullMetricsProvider : IMetricsProvider
    {
        public ICounter[] Counters => Array.Empty<ICounter>();
        
        public ICounter GetCounter(string name)
        {
            throw Error.NotFound($"Counter with name '{name}' isn't registered");
        }
    }
}