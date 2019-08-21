using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;

namespace Velo.Benchmark.Utf8Json
{
    public sealed class Utf8JsonConverter<T>
    {
        private readonly Func<T> _activator;
        private readonly Dictionary<string, SetDelegate> _setters;

        public Utf8JsonConverter(Func<T> activator)
        {
            _activator = Expression
                .Lambda<Func<T>>(Expression.New(typeof(T).GetConstructors()[0]))
                .Compile();
            
        }

        public T Deserialize(byte[] byteArray)
        {
            var instance = _activator();

            var reader = new Utf8JsonReader(byteArray);
            while (reader.Read())
            {
                var tokenType = reader.TokenType;
                
                if (tokenType == JsonTokenType.StartObject) continue;
                if (tokenType == JsonTokenType.EndObject) break;

                if (tokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    
                    reader.Read();
                    
                    if (reader.TokenType == JsonTokenType.Null) continue;
                    if (!_setters.TryGetValue(propertyName, out var setter)) continue;
                    
                    setter(ref reader, instance);
                }
            }

            return instance;
        }

        private delegate void SetDelegate(ref Utf8JsonReader reader, T instance);
    }
}