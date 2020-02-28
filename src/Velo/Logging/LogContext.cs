using System;
using System.IO;
using Velo.Logging.Formatters;
using Velo.Serialization.Models;

namespace Velo.Logging
{
    public readonly struct LogContext
    {
        public readonly LogLevel Level;
        public readonly Type Sender;
        public readonly string Template;

        private readonly ILogFormatter _formatter;

        internal LogContext(LogLevel level, Type sender, ILogFormatter formatter, string template)
        {
            Level = level;
            Sender = sender;
            _formatter = formatter;
            Template = template;
        }

        public string RenderMessage(JsonObject message)
        {
            using var stringWriter = new StringWriter();
            WriteMessage(message, stringWriter);
            return stringWriter.ToString();
        }
        
        public void WriteMessage(JsonObject message, TextWriter output)
        {
            if (_formatter == null)
            {
                DefaultStringFormatter.WritePrefixes(message, output);
                output.Write(Template);
            }
            else _formatter.Write(message, output);
        }
    }
}