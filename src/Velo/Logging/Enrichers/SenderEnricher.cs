using System;
using System.Collections.Concurrent;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Enrichers
{
    internal sealed class SenderEnricher : ConcurrentDictionary<Type, JsonVerbose>, ILogEnricher
    {
        public const string Name = "_sender";
        
        private readonly Func<Type, JsonVerbose> _builder;
        
        public SenderEnricher()
        {
            _builder = sender => new JsonVerbose(ReflectionUtils.GetName(sender));
        }

        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            var value = GetOrAdd(sender, _builder);
            message.Add(Name, value);
        }
    }
}