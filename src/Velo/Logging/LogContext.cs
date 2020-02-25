using System;

namespace Velo.Logging
{
    public readonly struct LogContext
    {
        public readonly LogLevel Level;
        public readonly Type Sender;
        public readonly string Template;

        public LogContext(LogLevel level, Type sender, string template)
        {
            Level = level;
            Sender = sender;
            Template = template;
        }
    }
}