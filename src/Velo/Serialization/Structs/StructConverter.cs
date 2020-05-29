using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Velo.Extensions;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Structs
{
    internal sealed class StructConverter<T> : JsonConverter<T>
        where T : struct
    {
        private readonly Func<T> _activator;
        private readonly Dictionary<string, StructPropertyConverter<T>> _properties;

        public StructConverter(IConvertersCollection converters) : base(false)
        {
            var type = Typeof<T>.Raw;
            _activator = Expression.Lambda<Func<T>>(Expression.New(type)).Compile();

            _properties = SerializationUtils.ActivateStructPropertyConverters<T>(converters);
        }

        public override T Deserialize(JsonTokenizer tokenizer)
        {
            var instance = _activator();

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
                if (!_properties.TryGetValue(propertyName, out var converter)) continue;

                converter.Deserialize(tokenizer, ref instance);
            }

            return instance;
        }

        public override T Read(JsonData jsonData)
        {
            var jsonObject = (JsonObject) jsonData;

            var instance = _activator();
            foreach (var converter in _properties.Values)
            {
                converter.Read(jsonObject, ref instance);
            }

            return instance;
        }

        public override void Serialize(T instance, TextWriter output)
        {
            output.Write('{');

            var first = true;
            foreach (var (name, converter) in _properties)
            {
                if (first) first = false;
                else output.Write(',');

                output.WriteProperty(name);
                converter.Serialize(ref instance, output);
            }

            output.Write('}');
        }

        public override JsonData Write(T instance)
        {
            var jsonObject = new JsonObject(_properties.Count);
            foreach (var converter in _properties.Values)
            {
                converter.Write(ref instance, jsonObject);
            }

            return jsonObject;
        }
    }
}