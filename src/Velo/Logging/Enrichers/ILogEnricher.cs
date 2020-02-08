using System;
using System.IO;

namespace Velo.Logging.Enrichers
{
    public interface ILogEnricher
    {
        void Enrich(TextWriter writer, LogLevel level, Type sender);
    }
}