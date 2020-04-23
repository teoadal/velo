using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Velo.Serialization.Converters;

namespace Velo.Metrics.Counters
{
    public static class CounterExtensions
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        
        public static Dictionary<string, double> Flush(this ICounter counter)
        {
            return Extract(counter, label => label.Flush());
        }

        public static void Flush(this ICounter counter, TextWriter writer)
        {
            Extract(counter, writer, label => label.Flush());
        }

        public static Dictionary<string, double> Read(this ICounter counter)
        {
            return Extract(counter, label => label.Value);
        }

        public static void Read(this ICounter counter, TextWriter writer)
        {
            Extract(counter, writer, label => label.Value);
        }

        private static Dictionary<string, double> Extract(ICounter counter, Func<ICounterLabel, double> extractor)
        {
            var labels = counter.Labels;
            var result = new Dictionary<string, double>(labels.Length);

            foreach (var label in labels)
            {
                result[label.Name] = extractor(label);
            }

            return result;
        }

        private static void Extract(ICounter counter, TextWriter writer, Func<ICounterLabel, double> extractor)
        {
            var labels = counter.Labels;

            writer.Write('{');

            var first = true;
            foreach (var label in labels)
            {
                if (first) first = false;
                else writer.Write(',');

                writer.Write('"');
                writer.Write(label.Name);
                writer.Write("\":");

                var value = extractor(label);
                writer.Write(value.ToString(DoubleConverter.Pattern, Invariant));
            }

            writer.Write('}');
        }
    }
}