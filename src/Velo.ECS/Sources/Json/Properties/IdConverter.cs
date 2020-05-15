using System.IO;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.Properties
{
    internal sealed class IdConverter : IPropertyConverter<IEntity>
    {
        private readonly SourceDescriptions _descriptions;

        private readonly IJsonConverter<int> _intConverter;
        private readonly IJsonConverter<string> _stringConverter;

        public IdConverter(IConvertersCollection converters, SourceDescriptions descriptions)
        {
            _descriptions = descriptions;
            _intConverter = converters.Get<int>();
            _stringConverter = converters.Get<string>();
        }

        public object? ReadValue(JsonObject source)
        {
            var value = (JsonValue) source[nameof(IEntity.Id)];
            if (value.Type == JsonDataType.Number)
            {
                return _intConverter.Read(value);
            }

            var alias = _stringConverter.Read(value);
            return _descriptions.GetOrAddAlias(alias);
        }

        public void Serialize(IEntity instance, TextWriter output)
        {
            var instanceId = instance.Id;

            if (_descriptions.TryGetAlias(instanceId, out var alias))
            {
                output.WriteString(alias);
            }
            else
            {
                output.Write(instanceId);
            }
        }

        public void Write(IEntity instance, JsonObject output)
        {
            var instanceId = instance.Id;

            var idValue = _descriptions.TryGetAlias(instanceId, out var alias)
                ? JsonValue.String(alias)
                : JsonValue.Number(instanceId);

            output.Add(nameof(IEntity.Id), idValue);
        }

        void IPropertyConverter<IEntity>.Read(JsonObject _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Deserialize(JsonTokenizer _, IEntity entity) => throw Error.NotSupported();
    }
}