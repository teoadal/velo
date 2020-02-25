using System;
using Velo.Serialization.Models;

namespace Velo.Logging.Enrichers
{
    public interface ILogEnricher
    {
        void Enrich(LogLevel level, Type sender, JsonObject message);
    }
}