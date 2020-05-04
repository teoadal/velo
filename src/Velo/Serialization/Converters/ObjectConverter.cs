using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Extensions;
using Velo.Serialization.Attributes;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal interface IObjectConverter : IJsonConverter
    {
        /// <summary>
        /// Fill present instance from json data 
        /// </summary>
        /// <returns>If token is <see cref="JsonTokenType.Null"/> - return null</returns>
        public object FillObject(JsonData jsonData, object instance);
    }

    internal sealed class ObjectConverter<T> : JsonConverter<T>, IObjectConverter
    {
        private readonly Func<T> _activator;
        private readonly EqualityComparer<T> _equalityComparer;
        private readonly Dictionary<string, PropertyConverter<T>> _propertyConverters;

        public ObjectConverter(ConvertersCollection converters) : base(false)
        {
            _activator = ExpressionUtils.BuildActivator<T>(throwIfEmptyConstructorNotFound: false);
            _equalityComparer = EqualityComparer<T>.Default;

            var properties = typeof(T).GetProperties();
            var propertyConverters = new Dictionary<string, PropertyConverter<T>>(
                properties.Length,
                StringUtils.IgnoreCaseComparer);

            foreach (var property in properties)
            {
                if (IgnoreAttribute.IsDefined(property)) continue;

                var propertyConverter = ConverterAttribute.IsDefined(property)
                    ? converters.GetCustomConverter(property.GetCustomAttribute<ConverterAttribute>().ConverterType)
                    : converters.Get(property.PropertyType);

                propertyConverters.Add(property.Name, new PropertyConverter<T>(property, propertyConverter));
            }

            _propertyConverters = propertyConverters;
        }

        public override T Deserialize(JsonTokenizer tokenizer)
        {
            var instance = _activator();
            return VisitProperties(tokenizer, instance);
        }

        public override T Read(JsonData jsonData)
        {
            if (jsonData.Type == JsonDataType.Null) return default!;

            var instance = _activator();
            VisitProperties((JsonObject) jsonData, instance);

            return instance;
        }

        public override void Serialize(T instance, TextWriter writer)
        {
            if (_equalityComparer.Equals(instance, default!))
            {
                writer.Write(JsonValue.NullToken);
                return;
            }

            writer.Write('{');

            var first = true;
            foreach (var (name, converter) in _propertyConverters)
            {
                if (first) first = false;
                else writer.Write(',');

                writer.Write('"');
                writer.Write(name);
                writer.Write("\":");
                converter.Serialize(instance, writer);
            }

            writer.Write('}');
        }

        public override JsonData Write(T instance)
        {
            if (_equalityComparer.Equals(instance, default!))
            {
                return JsonValue.Null;
            }

            var jsonObject = new JsonObject(_propertyConverters.Count);
            foreach (var (propertyName, converter) in _propertyConverters)
            {
                var propertyValue = converter.Write(instance);
                jsonObject.Add(propertyName, propertyValue);
            }

            return jsonObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VisitProperties(JsonObject jsonObject, T instance)
        {
            foreach (var (property, value) in jsonObject)
            {
                if (!_propertyConverters.TryGetValue(property, out PropertyConverter<T> converter)) continue;

                converter.Read(instance, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T VisitProperties(JsonTokenizer tokenizer, T instance)
        {
            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                var tokenType = token.TokenType;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (tokenType == JsonTokenType.ObjectStart) continue;
                if (tokenType == JsonTokenType.ObjectEnd) break;
                if (tokenType == JsonTokenType.Null) return default!;

                var propertyName = token.GetNotNullPropertyName();

                tokenizer.MoveNext(); // to property value

                if (tokenizer.Current.TokenType == JsonTokenType.Null) continue;
                if (!_propertyConverters.TryGetValue(propertyName!, out PropertyConverter<T> converter)) continue;

                converter.Deserialize(instance, tokenizer);
            }

            return instance;
        }

        object IObjectConverter.FillObject(JsonData data, object instance)
        {
            if (data.Type == JsonDataType.Null) return default!;
            VisitProperties((JsonObject) data, (T) instance);

            return instance;
        }
    }
}