using System;
using System.Threading;
using Velo.Serialization.Models;

namespace Velo.Logging.Enrichers
{
    internal sealed class TimeStampEnricher : ILogEnricher
    {
        private const string Name = "_timestamp";

        private DateTime _last;
        private JsonVerbose _lastVerbose;
        private SpinLock _lock;

        public TimeStampEnricher()
        {
            _lock = new SpinLock();
            
            GetTimestamp();
        }

        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            message.Add(Name, GetTimestamp());
        }

        private JsonVerbose GetTimestamp()
        {
            var current = DateTime.UtcNow;

            JsonVerbose result;
            var lockTaken = false;
            
            _lock.Enter(ref lockTaken);

            if (current.Ticks - _last.Ticks < 1000) result = _lastVerbose;
            else
            {
                result = new JsonVerbose(current.ToString("s"));
                _last = current;
                _lastVerbose = result;    
            }
            
            if (lockTaken) _lock.Exit();
            
            return result;
        }
    }
}