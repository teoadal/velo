using System;
using System.IO;
using Velo.Utils;

namespace Velo.Logging.Enrichers
{
    internal sealed class SenderEnricher : ILogEnricher
    {
        public void Enrich(TextWriter writer, LogLevel level, Type sender)
        {
            writer.Write('[');
            ReflectionUtils.WriteName(sender, writer);
            writer.Write(']');
        }
    }
}