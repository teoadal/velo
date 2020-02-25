using System;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Serialization.Models;

namespace Velo.TestsModels.Logging
{
    public sealed class TestLogEnricher : ILogEnricher
    {
        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            message.Add(nameof(TestLogEnricher), new JsonVerbose(nameof(TestLogEnricher)));
        }
    }
}