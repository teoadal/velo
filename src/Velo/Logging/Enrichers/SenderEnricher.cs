using System;
using System.Collections.Concurrent;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Enrichers
{
    internal sealed class SenderEnricher : ILogEnricher
    {
        private const string Name = "_sender";

        private readonly Func<Type, JsonVerbose> _builder;
        private readonly ConcurrentDictionary<Type, JsonVerbose> _senders;

        public SenderEnricher()
        {
            _builder = Build;
            _senders = new ConcurrentDictionary<Type, JsonVerbose>();
        }

        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            var value = _senders.GetOrAdd(sender, _builder);
            message.Add(Name, value);
        }

        private static JsonVerbose Build(Type type)
        {
            return new JsonVerbose(ReflectionUtils.GetName(type));
        }
    }
}