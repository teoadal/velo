using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Velo.Extensions;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Objects
{
    internal sealed class ObjectConverter<T> : JsonConverter<T>, IObjectConverter<T>
    {
        private readonly Func<T> _activator;
        private readonly EqualityComparer<T> _equalityComparer;
        private readonly Dictionary<string, IPropertyConverter<T>> _properties;

        public ObjectConverter(IServiceProvider services, IConvertersCollection converters)
            : base(false)
        {
            _activator = ExpressionUtils.BuildActivator<T>(throwIfEmptyConstructorNotFound: false);
            _equalityComparer = EqualityComparer<T>.Default;
            _properties = PropertyConverter<T>.CreateCollection(services, converters);
        }

        public override T Deserialize(JsonTokenizer tokenizer)
        {
            var instance = _activator();
            return VisitProperties(tokenizer, instance);
        }

        public T Fill(JsonObject jsonData, T instance)
        {
            if (jsonData.Type == JsonDataType.Null) return default!;

            VisitProperties(jsonData, instance);

            return instance;
        }

        public override T Read(JsonData jsonData)
        {
            if (jsonData.Type == JsonDataType.Null) return default!;

            var instance = _activator();
            VisitProperties((JsonObject) jsonData, instance);

            return instance;
        }

        public override void Serialize(T instance, TextWriter output)
        {
            if (_equalityComparer.Equals(instance, default!))
            {
                output.Write(JsonValue.NullToken);
                return;
            }

            output.Write('{');

            var first = true;
            foreach (var (name, converter) in _properties)
            {
                if (first) first = false;
                else output.Write(',');

                output.WriteProperty(name);
                converter.Serialize(instance, output);
            }

            output.Write('}');
        }

        public override JsonData Write(T instance)
        {
            if (_equalityComparer.Equals(instance, default!))
            {
                return JsonValue.Null;
            }

            var jsonObject = new JsonObject(_properties.Count);
            foreach (var converter in _properties.Values)
            {
                converter.Write(instance, jsonObject);
            }

            return jsonObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VisitProperties(JsonObject jsonObject, T instance)
        {
            foreach (var converter in _properties.Values)
            {
                converter.Read(jsonObject, instance);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T VisitProperties(JsonTokenizer tokenizer, T instance)
        {
            // it's maybe different tokens
            if (tokenizer.Current.TokenType == JsonTokenType.None) tokenizer.MoveNext();
            if (tokenizer.Current.TokenType == JsonTokenType.Null) return default!;
            
            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                var tokenType = token.TokenType;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (tokenType == JsonTokenType.ObjectStart) continue;
                if (tokenType == JsonTokenType.ObjectEnd) break;

                var propertyName = token.GetNotNullPropertyName();

                tokenizer.MoveNext(); // to property value

                if (tokenizer.Current.TokenType == JsonTokenType.Null) continue;
                if (!_properties.TryGetValue(propertyName, out IPropertyConverter<T> converter)) continue;

                converter.Deserialize(tokenizer, instance);
            }

            return instance;
        }

        object? IObjectConverter.FillObject(JsonObject jsonData, object instance) => Fill(jsonData, (T) instance);
    }
}