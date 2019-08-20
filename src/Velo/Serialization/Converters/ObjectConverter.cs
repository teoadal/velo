using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Velo.Serialization.Converters
{
    internal sealed class ObjectConverter<T> : IJsonConverter<T>
    {
        private readonly Func<T> _activator;
        private readonly EqualityComparer<T> _equalityComparer;
        private readonly Dictionary<string, Action<T, JsonTokenizer>> _deserializers;
        private readonly Dictionary<string, Action<T, StringBuilder>> _serializers;

        public ObjectConverter(JConverter converter)
        {
            var outType = typeof(T);
            var properties = outType.GetProperties();
            var outInstanceConstructor = outType.GetConstructor(Array.Empty<Type>());

            _activator = outInstanceConstructor == null
                ? throw new Exception($"Default constructor for {outType.Name} not found")
                : Expression.Lambda<Func<T>>(Expression.New(outInstanceConstructor)).Compile();

            _deserializers = properties.ToDictionary(
                p => p.Name,
                p => BuildPropertyDeserializer(p, converter.GetConverter(p.PropertyType)));

            _equalityComparer = EqualityComparer<T>.Default;

            _serializers = properties.ToDictionary(
                p => p.Name,
                p => BuildPropertySerializer(p, converter.GetConverter(p.PropertyType)));
        }

        public T Deserialize(JsonTokenizer tokenizer)
        {
            var outInstance = _activator();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                var tokenType = current.TokenType;

                if (tokenType == JsonTokenType.Null) return default;
                if (tokenType == JsonTokenType.ObjectStart) continue;
                if (tokenType == JsonTokenType.ObjectEnd) break;

                if (tokenType != JsonTokenType.Property)
                {
                    throw new InvalidCastException($"Invalid token '{current}' in object");
                }

                if (_deserializers.TryGetValue(current.Value, out var converter))
                {
                    tokenizer.MoveNext();
                    if (tokenizer.Current.TokenType == JsonTokenType.Null) continue;
                    converter(outInstance, tokenizer);
                }
            }

            return outInstance;
        }

        public void Serialize(T value, StringBuilder builder)
        {
            if (_equalityComparer.Equals(value, default))
            {
                builder.Append(JsonTokenizer.NullValue);
                return;
            }

            builder.Append('{');

            var first = true;
            foreach (var pair in _serializers)
            {
                if (first) first = false;
                else builder.Append(',');

                builder.Append('"').Append(pair.Key).Append("\":");
                pair.Value(value, builder);
            }

            builder.Append('}');
        }

        private static Action<T, StringBuilder> BuildPropertySerializer(PropertyInfo property,
            IJsonConverter jsonConverter)
        {
            const string serializeMethodName = nameof(IJsonConverter<object>.Serialize);

            var instance = Expression.Parameter(typeof(T), "instance");
            var builder = Expression.Parameter(typeof(StringBuilder), "builder");

            var converterType = jsonConverter.GetType();
            var serializeMethod = converterType.GetMethod(serializeMethodName);

            if (serializeMethod == null)
            {
                throw new InvalidOperationException($"Bad converter for type {property.PropertyType}");
            }

            var converter = Expression.Constant(jsonConverter, converterType);
            var propertyValue = Expression.Property(instance, property);

            var body = Expression.Call(converter, serializeMethod, propertyValue, builder);
            return Expression
                .Lambda<Action<T, StringBuilder>>(body, instance, builder)
                .Compile();
        }

        private static Action<T, JsonTokenizer> BuildPropertyDeserializer(PropertyInfo property,
            IJsonConverter jsonConverter)
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var instance = Expression.Parameter(typeof(T), "instance");
            var tokenizer = Expression.Parameter(typeof(JsonTokenizer), "tokenizer");

            var converterType = jsonConverter.GetType();
            var deserializeMethod = converterType.GetMethod(deserializeMethodName);

            if (deserializeMethod == null)
            {
                throw new InvalidOperationException($"Bad converter for type {property.PropertyType}");
            }

            var converter = Expression.Constant(jsonConverter, converterType);
            var propertyValue = Expression.Call(converter, deserializeMethod, tokenizer);

            var body = Expression.Assign(Expression.Property(instance, property), propertyValue);
            return Expression
                .Lambda<Action<T, JsonTokenizer>>(body, instance, tokenizer)
                .Compile();
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((T) value, builder);
    }
}