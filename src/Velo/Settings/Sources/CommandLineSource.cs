using Velo.Collections;
using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal sealed class CommandLineSource : ISettingsSource
    {
        private readonly JsonObject _configuration;

        public CommandLineSource(string[] args)
        {
            if (args.NullOrEmpty()) return;

            _configuration = new JsonObject();
            for (var i = 0; i < args.Length; i++)
            {
                ref readonly var element = ref args[i];
                var verbatim = element.StartsWith("--");

                var key = verbatim ? element.Substring(2) : element;

                var value = string.Empty;
                if (verbatim)
                {
                    value = i < args.Length - 1 ? args[++i] : null;
                }
                else
                {
                    var valueIndex = element.IndexOf('=');
                    if (valueIndex > -1)
                    {
                        key = element.Substring(0, valueIndex);
                        value = element.Substring(valueIndex + 1);
                    }
                }

                var jsonValue = VisitValue(value);

                var dotIndex = key.IndexOf('.');
                if (dotIndex == -1) _configuration.Add(key, jsonValue);
                else SetValueByPath(_configuration, key, jsonValue);
            }
        }

        public bool TryGet(out JsonObject data)
        {
            data = _configuration;
            return _configuration != null;
        }

        private static void SetValueByPath(JsonObject node, string path, JsonData value)
        {
            var start = 0;

            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] != '.') continue;

                var property = path.Substring(start, i - start);

                if (!node.TryGet(property, out var propertyNode))
                {
                    propertyNode = new JsonObject();
                    node[property] = propertyNode;
                }

                node = (JsonObject) propertyNode;
                start = i + 1;
            }

            var valueProperty = path.Substring(start, path.Length - start);
            node[valueProperty] = value;
        }

        private static JsonData VisitValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return JsonValue.Null;

            var firstChar = value[0];

            if (char.IsDigit(firstChar))
            {
                return value.Length == 1 && firstChar == '0'
                    ? JsonValue.Zero
                    : new JsonValue(value, JsonDataType.Number);
            }

            switch (firstChar)
            {
                case '"':
                    return JsonValue.String(value.Substring(1, value.Length - 2));
                case 'f':
                    return JsonValue.False;
                case 't':
                    return JsonValue.True;
                case 'n':
                    return JsonValue.Null;
            }

            return JsonValue.String(value);
        }
    }
}