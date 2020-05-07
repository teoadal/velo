using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections.Local;
using Velo.Extensions;
using Velo.Serialization.Models;
using Velo.Text;

namespace Velo.Logging.Formatters
{
    internal sealed class DefaultStringFormatter : ILogFormatter
    {
        private readonly Part[] _parts;

        public DefaultStringFormatter(string template)
        {
            var parts = new LocalList<Part>();
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

            _parts = parts.ToArray();
        }

        public void Write(JsonObject message, TextWriter output)
        {
            WritePrefixes(message, output);

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

        internal static void WritePrefixes(JsonObject message, TextWriter writer)
        {
            foreach (var (propertyName, jsonData) in message)
            {
                if (propertyName[0] != '_') continue;
                writer.Write('[');
                jsonData.Serialize(writer);
                writer.Write("] ");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Part BuildPart(StringBuilder buffer, bool isArgument)
        {
            var value = StringUtils.Release(buffer);
            return new Part(isArgument, value);
        }

        private readonly struct Part
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
}