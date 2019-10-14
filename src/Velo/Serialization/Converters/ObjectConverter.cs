using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Velo.Extensions;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal sealed class ObjectConverter<TObject> : IJsonConverter<TObject>
    {
        public bool IsPrimitive => false;
        
        private readonly Func<TObject> _activator;
        private readonly EqualityComparer<TObject> _equalityComparer;
        private readonly Dictionary<string, Action<TObject, JsonTokenizer>> _deserializeMethods;
        private readonly Dictionary<string, Action<TObject, StringBuilder>> _serializeMethods;

        public ObjectConverter(Dictionary<PropertyInfo, IJsonConverter> propertyConverters)
        {
            var outInstanceConstructor = typeof(TObject).GetConstructor(Array.Empty<Type>());

            _activator = outInstanceConstructor == null
                ? throw Error.DefaultConstructorNotFound(typeof(TObject))
                : Expression.Lambda<Func<TObject>>(Expression.New(outInstanceConstructor)).Compile();

            _deserializeMethods = propertyConverters.ToDictionary(
                pair => pair.Key.Name,
                pair => BuildDeserializeMethod(pair.Key, pair.Value));

            _equalityComparer = EqualityComparer<TObject>.Default;

            _serializeMethods = propertyConverters.ToDictionary(
                pair => pair.Key.Name,
                pair => BuildSerializeMethod(pair.Key, pair.Value));
        }

        public TObject Deserialize(JsonTokenizer tokenizer)
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
                    throw new InvalidCastException($"Invalid token '{token}' in object");
                }

                var propertyName = token.Value;

                tokenizer.MoveNext(); // to property value

                if (tokenizer.Current.TokenType == JsonTokenType.Null) continue;
                if (!_deserializeMethods.TryGetValue(propertyName, out var converter)) continue;

                converter(instance, tokenizer);
            }

            return instance;
        }

        public void Serialize(TObject instance, StringBuilder builder)
        {
            if (_equalityComparer.Equals(instance, default))
            {
                builder.Append(JsonTokenizer.TOKEN_NULL_VALUE);
                return;
            }

            builder.Append('{');

            var first = true;
            foreach (var (name, action) in _serializeMethods)
            {
                if (first) first = false;
                else builder.Append(',');

                builder.Append('"').Append(name).Append("\":");
                action(instance, builder);
            }

            builder.Append('}');
        }

        private static Action<TObject, StringBuilder> BuildSerializeMethod(PropertyInfo property,
            IJsonConverter propertyValueConverter)
        {
            const string serializeMethodName = nameof(IJsonConverter<object>.Serialize);

            var instance = Expression.Parameter(typeof(TObject), "instance");
            var builder = Expression.Parameter(typeof(StringBuilder), "builder");

            var converterType = propertyValueConverter.GetType();
            var serializeMethod = converterType.GetMethod(serializeMethodName);

            var converter = Expression.Constant(propertyValueConverter, converterType);
            var propertyValue = Expression.Property(instance, property);

            // ReSharper disable once AssignNullToNotNullAttribute
            var body = Expression.Call(converter, serializeMethod, propertyValue, builder);
            return Expression
                .Lambda<Action<TObject, StringBuilder>>(body, instance, builder)
                .Compile();
        }

        private static Action<TObject, JsonTokenizer> BuildDeserializeMethod(PropertyInfo property,
            IJsonConverter propertyValueConverter)
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var instance = Expression.Parameter(typeof(TObject), "instance");
            var tokenizer = Expression.Parameter(typeof(JsonTokenizer), "tokenizer");

            var converterType = propertyValueConverter.GetType();
            var deserializeMethod = converterType.GetMethod(deserializeMethodName);

            var converter = Expression.Constant(propertyValueConverter, converterType);
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyValue = Expression.Call(converter, deserializeMethod, tokenizer);

            var body = Expression.Assign(Expression.Property(instance, property), propertyValue);
            return Expression
                .Lambda<Action<TObject, JsonTokenizer>>(body, instance, tokenizer)
                .Compile();
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TObject) value, builder);
    }
}