using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private readonly Dictionary<string, PropertyConverter> _propertyConverters;

        public ObjectConverter(Dictionary<PropertyInfo, IJsonConverter> propertyConverters)
        {
            var outInstanceConstructor = typeof(TObject).GetConstructor(Array.Empty<Type>());

            _activator = outInstanceConstructor == null
                ? throw Error.DefaultConstructorNotFound(typeof(TObject))
                : Expression.Lambda<Func<TObject>>(Expression.New(outInstanceConstructor)).Compile();

            _equalityComparer = EqualityComparer<TObject>.Default;
            
            _propertyConverters = new Dictionary<string, PropertyConverter>(propertyConverters.Count);
            foreach (var (propertyInfo, converter) in propertyConverters)
            {
                var propertyName = propertyInfo.Name;
                var propertyConverter = new PropertyConverter(
                    BuildDeserializeMethod(propertyInfo, converter),
                    BuildReadMethod(propertyInfo, converter),
                    BuildSerializeMethod(propertyInfo, converter));
                
                _propertyConverters.Add(propertyName, propertyConverter);
            }
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

        private static DeserializeMethod BuildDeserializeMethod(PropertyInfo property,
            IJsonConverter propertyValueConverter)
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var instance = Expression.Parameter(typeof(TObject), "instance");
            var tokenizer = Expression.Parameter(typeof(JsonTokenizer).MakeByRefType(), "tokenizer");

            var converterType = propertyValueConverter.GetType();
            var deserializeMethod = converterType.GetMethod(deserializeMethodName);

            var converter = Expression.Constant(propertyValueConverter, converterType);
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyValue = Expression.Call(converter, deserializeMethod, tokenizer);

            var body = Expression.Assign(Expression.Property(instance, property), propertyValue);
            return Expression
                .Lambda<DeserializeMethod>(body, instance, tokenizer)
                .Compile();
        }

        private static Action<TObject, JsonData> BuildReadMethod(PropertyInfo property,
            IJsonConverter propertyValueConverter)
        {
            const string readMethodName = nameof(IJsonConverter<object>.Read);

            var instance = Expression.Parameter(typeof(TObject), "instance");
            var tokenizer = Expression.Parameter(typeof(JsonData), "data");

            var converterType = propertyValueConverter.GetType();
            var deserializeMethod = converterType.GetMethod(readMethodName);

            var converter = Expression.Constant(propertyValueConverter, converterType);
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyValue = Expression.Call(converter, deserializeMethod, tokenizer);

            var body = Expression.Assign(Expression.Property(instance, property), propertyValue);
            return Expression
                .Lambda<Action<TObject, JsonData>>(body, instance, tokenizer)
                .Compile();
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
        
        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TObject) value, builder);

        private delegate void DeserializeMethod(TObject obj, ref JsonTokenizer tokenizer);
        
        private readonly struct PropertyConverter
        {
            public readonly DeserializeMethod Deserialize;
            public readonly Action<TObject, JsonData> Read;
            public readonly Action<TObject, StringBuilder> Serialize;

            public PropertyConverter(DeserializeMethod deserialize, Action<TObject, JsonData> read, Action<TObject, StringBuilder> serialize)
            {
                Deserialize = deserialize;
                Read = read;
                Serialize = serialize;
            }
        }
    }
}