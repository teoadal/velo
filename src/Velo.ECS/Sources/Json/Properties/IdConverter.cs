using System.IO;
using System.Reflection;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.Properties
{
    internal sealed class IdConverter : IPropertyConverter<IEntity>
    {
        private readonly IJsonConverter<int> _intConverter;
        private readonly IJsonConverter<string> _stringConverter;
        private readonly string _propertyName;

        public IdConverter(PropertyInfo property, IConvertersCollection converters)
        {
            _propertyName = property.Name;

            _intConverter = converters.Get<int>();
            _stringConverter = converters.Get<string>();
        }

        public object? ReadValue(JsonObject source)
        {
            var value = (JsonValue) source[_propertyName];
            if (value.Type == JsonDataType.Number)
            {
                return _intConverter.Read(value);
            }

            var alias = _stringConverter.Read(value);
            return SourceDescriptions.GetOrAddAlias(alias);
        }

        public void Serialize(IEntity instance, TextWriter output)
        {
            var instanceId = instance.Id;

            if (SourceDescriptions.TryGetAlias(instanceId, out var alias))
            {
                output.WriteString(alias);
            }
            else
            {
                output.Write(instanceId);
            }
        }

        void IPropertyConverter<IEntity>.Read(JsonObject _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Deserialize(JsonTokenizer _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Write(IEntity instance, JsonObject _) => throw Error.NotSupported();
    }
}