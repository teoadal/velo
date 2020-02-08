using System.Collections.Generic;
using Velo.Logging;

namespace Velo.TestsModels.Logging
{
    public class ListLoggerWriter : ILogWriter
    {
        public LogLevel Level => LogLevel.None;

        public readonly IReadOnlyList<string> Messages;

        private readonly List<string> _messages;

        public ListLoggerWriter()
        {
            _messages = new List<string>();
            Messages = _messages;
        }
        
        public void Write(LogLevel level, string message)
        {
            _messages.Add(message);
        }
    }
}