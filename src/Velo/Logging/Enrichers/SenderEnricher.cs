using System;
using System.Collections.Generic;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Enrichers
{
    internal sealed class SenderEnricher : ILogEnricher
    {
        public const string Name = "_sender";

        private readonly object _lock;
        private readonly Dictionary<Type, JsonVerbose> _senders;

        public SenderEnricher()
        {
            _lock = new object();
            _senders = new Dictionary<Type, JsonVerbose>();
        }

        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            if (!_senders.TryGetValue(sender, out var value))
            {
                value = new JsonVerbose(ReflectionUtils.GetName(sender));
                using (Lock.Enter(_lock))
                {
                    _senders[sender] = value;
                }
            }

            message.Add(Name, value);
        }
    }
}