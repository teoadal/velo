using System;
using System.IO;
using Velo.Metrics.Counters;

namespace Velo.Metrics.Provider
{
    public static class MetricServiceExtensions
    {
        public static void Flush(this IMetricsProvider metrics, TextWriter writer)
        {
            Extract(metrics, writer, (counter, textWriter) => counter.Flush(textWriter));
        }


        public static void Read(this IMetricsProvider metrics, TextWriter writer)
        {
            Extract(metrics, writer, (counter, textWriter) => counter.Read(textWriter));
        }

        private static void Extract(IMetricsProvider metrics, TextWriter writer, Action<ICounter, TextWriter> extractor)
        {
            var first = true;
            foreach (var counter in metrics.Counters)
            {
                if (first) first = false;
                else writer.Write(',');

                writer.Write('"');
                writer.Write(counter.Name);
                writer.Write("\":");

                extractor(counter, writer);
            }
        }
    }
}