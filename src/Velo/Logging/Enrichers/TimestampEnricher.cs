using System;
using System.IO;

namespace Velo.Logging.Enrichers
{
    internal sealed class TimestampEnricher : ILogEnricher
    {
        public void Enrich(TextWriter writer, LogLevel level, Type sender)
        {
            var now = DateTime.Now;
            writer.Write('[');
            writer.Write(now.ToShortTimeString());
            writer.Write(']');
        }
    }
}