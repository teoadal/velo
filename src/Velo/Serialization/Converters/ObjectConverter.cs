using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal sealed class ObjectConverter<TObject> : IJsonConverter<TObject>
    {
        public bool IsPrimitive => false;

        private readonly Func<TObject> _activator;
        private readonly EqualityComparer<TObject> _equalityComparer;
        private readonly Dictionary<string, PropertyConverter<TObject>> _propertyConverters;

        public ObjectConverter((PropertyInfo, IJsonConverter)[] propertyConverters)
        {
            _activator = ExpressionUtils.BuildActivator<TObject>();
            _equalityComparer = EqualityComparer<TObject>.Default;

            var converters = new Dictionary<string, PropertyConverter<TObject>>(propertyConverters.Length);
            foreach (var (propertyInfo, converter) in propertyConverters)
            {
                var propertyName = propertyInfo.Name;
                var propertyConverter = new PropertyConverter<TObject>(propertyInfo, converter);

                converters.Add(propertyName, propertyConverter);
            }
            _propertyConverters = converters;
        }

        public TObject Deserialize(ref JsonTokenizer tokenizer)
        {
            var instance = _activator();

            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                var tokenType = token.TokenType;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (tokenType == JsonTokenType.Null) return default;
                if (tokenType == JsonTokenType.ObjectStart) continue;
                if (tokenType == JsonTokenType.ObjectEnd) break;

                if (tokenType != JsonTokenType.Property)
                {
                    throw new InvalidCastException($"Invalid token '{token.TokenType}' in object");
                }

                var propertyName = token.Value;

                tokenizer.MoveNext(); // to property value

                if (tokenizer.Current.TokenType == JsonTokenType.Null) continue;
                if (!_propertyConverters.TryGetValue(propertyName, out var converter)) continue;

                converter.Deserialize(instance, ref tokenizer);
            }

            return instance;
        }

        public TObject Read(JsonData jsonData)
        {
            var instance = _activator();

            var objectData = (JsonObject) jsonData;
            foreach (var (property, value) in objectData)
            {
                if (!_propertyConverters.TryGetValue(property, out var converter)) continue;

                converter.Read(instance, value);
            }

            return instance;
        }

        public void Serialize(TObject instance, StringBuilder builder)
        {
            if (_equalityComparer.Equals(instance, default))
            {
                builder.Append(JsonTokenizer.TokenNullValue);
                return;
            }

            builder.Append('{');

            var first = true;
            foreach (var (name, converter) in _propertyConverters)
            {
                if (first) first = false;
                else builder.Append(',');

                builder.Append('"').Append(name).Append("\":");
                converter.Serialize(instance, builder);
            }

            builder.Append('}');
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TObject) value, builder);
    }
}