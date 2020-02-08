using System;
using System.IO;

namespace Velo.Logging.Enrichers
{
    internal sealed class LogLevelEnricher : ILogEnricher
    {
        public void Enrich(TextWriter writer, LogLevel level, Type sender)
        {
            writer.Write('[');

            switch (level)
            {
                case LogLevel.Verbose:
                    writer.Write("VRB");
                    break;
                case LogLevel.Debug:
                    writer.Write("DBG");
                    break;
                case LogLevel.Info:
                    writer.Write("INF");
                    break;
                case LogLevel.Warning:
                    writer.Write("WRN");
                    break;
                case LogLevel.Error:
                    writer.Write("ERR");
                    break;
            }
            
            
            writer.Write(']');
        }
    }
}