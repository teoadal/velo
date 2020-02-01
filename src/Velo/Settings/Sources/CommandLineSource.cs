using System;
using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal sealed class CommandLineSource : IConfigurationSource
    {
        private readonly JsonObject _configuration;

        public CommandLineSource(string[] args)
        {
            if (args == null) return;

            _configuration = new JsonObject();
            for (var i = 0; i < args.Length; i++)
            {
                ref readonly var element = ref args[i];
                var verbatim = element.StartsWith("--");

                var key = verbatim ? element.AsSpan(2) : element;

                var value = ReadOnlySpan<char>.Empty;
                if (verbatim)
                {
                    value = i < args.Length - 1 ? args[++i] : null;
                }
                else
                {
                    var valueIndex = element.IndexOf('=');
                    if (valueIndex > -1)
                    {
                        key = element.AsSpan(0, valueIndex);
                        value = element.AsSpan(valueIndex + 1);
                    }
                }

                var jsonValue = VisitValue(value);

                var dotIndex = key.IndexOf('.');
                if (dotIndex == -1) _configuration.Add(new string(value), jsonValue);
                else SetValueByPath(_configuration, key, jsonValue);
            }
        }

        public bool TryGet(out JsonObject data)
        {
            data = _configuration;
            return true;
        }

        private static void SetValueByPath(JsonObject node, ReadOnlySpan<char> path, JsonData value)
        {
            var start = 0;

            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] != '.') continue;

                var property = new string(path.Slice(start, i - start));

                if (!node.TryGet(property, out var propertyNode))
                {
                    propertyNode = new JsonObject();
                    node[property] = propertyNode;
                }

                node = (JsonObject) propertyNode;
                start = i + 1;
            }

            var valueProperty = new string(path.Slice(start, path.Length - start));
            node[valueProperty] = value;
        }

        private static JsonData VisitValue(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return JsonValue.Null;

            var firstChar = value[0];

            if (char.IsDigit(firstChar))
            {
                return value.Length == 1 && firstChar == '0'
                    ? JsonValue.Zero
                    : new JsonValue(new string(value), JsonDataType.Number);
            }

            switch (firstChar)
            {
                case '"':
                    return JsonValue.String(new string(value.Slice(1, value.Length - 2)));
                case 'f':
                    return JsonValue.False;
                case 't':
                    return JsonValue.True;
                case 'n':
                    return JsonValue.Null;
            }

            return JsonValue.String(new string(value));
        }
    }
}