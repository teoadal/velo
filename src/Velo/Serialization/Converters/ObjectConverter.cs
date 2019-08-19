using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Velo.Serialization.Converters
{
    internal sealed class ObjectConverter<T> : IJsonConverter<T>
    {
        private readonly Dictionary<string, Action<T, JsonTokenizer>> _converters;
        private readonly Func<T> _activator;

        public ObjectConverter(JSerializer serializer)
        {
            var outType = typeof(T);
            
            _converters = outType
                .GetProperties()
                .ToDictionary(p => p.Name, p => BuildPropertyConverter(p, serializer.GetOrBuildConverter(p.PropertyType)));
            
            var outInstanceConstructor = outType.GetConstructor(Array.Empty<Type>());
            _activator = outInstanceConstructor == null
                ? throw new Exception($"Default constructor for {outType.Name} not found")
                : Expression.Lambda<Func<T>>(Expression.New(outInstanceConstructor)).Compile();
        }

        public T Convert(JsonTokenizer tokenizer)
        {
            var outInstance = _activator();
            
            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                var tokenType = current.TokenType;
                
                if (tokenType == JTokenType.ObjectStart) continue;
                if (tokenType == JTokenType.ObjectEnd) break;

                if (tokenType != JTokenType.Property)
                {
                    throw new InvalidCastException($"Invalid token '{current}' in object");
                }

                if (_converters.TryGetValue(current.Value, out var converter))
                {
                    tokenizer.MoveNext();
                    converter(outInstance, tokenizer);
                }
            }

            return outInstance;
        }

        private static Action<T, JsonTokenizer> BuildPropertyConverter(PropertyInfo property, IJsonConverter converter)
        {
            const string convertMethodName = nameof(IJsonConverter<object>.Convert);
            
            var instance = Expression.Parameter(typeof(T), "instance");
            var tokenizer = Expression.Parameter(typeof(JsonTokenizer), "tokenizer");

            var converterType = converter.GetType();
            var typedConverter = Expression.Constant(converter, converterType);
            var convertMethod = converterType.GetMethod(convertMethodName);

            if (convertMethod == null)
            {
                throw new InvalidOperationException($"Bad converter for type {property.PropertyType}");
            }
            
            var assign = Expression.Assign(
                Expression.Property(instance, property),
                Expression.Call(typedConverter, convertMethod, tokenizer));

            return Expression
                .Lambda<Action<T, JsonTokenizer>>(assign, instance, tokenizer)
                .Compile();
        }
    }
}