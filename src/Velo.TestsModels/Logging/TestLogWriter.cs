using System;
using System.Collections.Generic;
using System.IO;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.TestsModels.Logging
{
    public class TestLogWriter : ILogWriter
    {
        public LogLevel Level { get; set; }

        public readonly IReadOnlyList<string> Messages;

        /// <summary>
        /// Can't check message by Moq.Verify - it's cleanup after write
        /// </summary>
        public Action<LogContext, JsonObject> Verify;

        private readonly List<string> _messages;

        public TestLogWriter()
        {
            Level = LogLevel.Debug;

            _messages = new List<string>();
            Messages = _messages;
        }

        public void Write(LogContext context, JsonObject message)
        {
            Verify?.Invoke(context, message);

            var stringWriter = new StringWriter();
            message.Serialize(stringWriter);
            _messages.Add(StringUtils.Release(stringWriter));
        }
    }
}