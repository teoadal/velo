using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Logging.Writers
{
    internal sealed partial class ConsoleLogWriter
    {
        private sealed class MessageWriter
        {
            private readonly Part[] _parts;

            public MessageWriter(Part[] parts)
            {
                _parts = parts;
            }

            public void Write(JsonObject message, TextWriter output)
            {
                foreach (var part in _parts)
                {
                    if (part.IsArgument)
                    {
                        var partValue = message[part.Value];
                        partValue.Serialize(output);
                    }
                    else
                    {
                        output.Write(part.Value);
                    }
                }
            }

            public readonly struct Part
            {
                public readonly bool IsArgument;
                public readonly string Value;

                public Part(bool isArgument, string value)
                {
                    IsArgument = isArgument;
                    Value = value;
                }
            }
        }

        private static MessageWriter BuildWriter(string template)
        {
            var parts = new LocalList<MessageWriter.Part>();
            var buffer = new StringBuilder();

            foreach (var ch in template)
            {
                switch (ch)
                {
                    case '{':
                        if (buffer.Length > 0) parts.Add(BuildPart(buffer, false));
                        break;
                    case '}':
                        parts.Add(BuildPart(buffer, true));
                        break;
                    default:
                        buffer.Append(ch);
                        break;
                }
            }

            if (buffer.Length > 0) parts.Add(BuildPart(buffer, false));

            return new MessageWriter(parts.ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MessageWriter.Part BuildPart(StringBuilder buffer, bool isArgument)
        {
            var value = StringUtils.Release(buffer);
            return new MessageWriter.Part(isArgument, value);
        }
    }
}