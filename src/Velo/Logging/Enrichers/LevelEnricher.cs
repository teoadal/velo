using System;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Enrichers
{
    internal sealed class LevelEnricher : ILogEnricher
    {
        private const string Name = "_level";

        private readonly JsonVerbose _errorVerbose;
        private readonly JsonVerbose _debugVerbose;
        private readonly JsonVerbose _infoVerbose;
        private readonly JsonVerbose _traceVerbose;
        private readonly JsonVerbose _warningVerbose;

        public LevelEnricher()
        {
            _errorVerbose = new JsonVerbose("ERR");
            _debugVerbose = new JsonVerbose("DBG");
            _infoVerbose = new JsonVerbose("INF");
            _traceVerbose = new JsonVerbose("TRA");
            _warningVerbose = new JsonVerbose("WRN");
        }
        
        public void Enrich(LogLevel level, Type sender, JsonObject message)
        {
            JsonVerbose value;
            switch (level)
            {
                case LogLevel.Trace:
                    value = _traceVerbose;
                    break;
                case LogLevel.Debug:
                    value = _debugVerbose;
                    break;
                case LogLevel.Info:
                    value = _infoVerbose;
                    break;
                case LogLevel.Warning:
                    value = _warningVerbose;
                    break;
                case LogLevel.Error:
                    value = _errorVerbose;
                    break;
                default:
                    throw Error.OutOfRange($"Log level '{level}' isn't supported");
            }
            
            message.Add(Name, value);
        }
    }
}