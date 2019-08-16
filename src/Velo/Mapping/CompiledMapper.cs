using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Velo.Mapping
{
    public sealed class CompiledMapper<TOut> : IMapper<TOut>
    {
        private readonly Dictionary<Type, Func<object, TOut>> _converters;
        private readonly ConstructorInfo _outConstructor;
        private readonly Dictionary<string, PropertyInfo> _outProperties;
        private readonly Type _outType;

        public CompiledMapper()
        {
            _converters = new Dictionary<Type, Func<object, TOut>>();

            _outType = typeof(TOut);
            _outProperties = _outType.GetProperties().ToDictionary(p => p.Name);
            _outConstructor = _outType.GetConstructor(Array.Empty<Type>());

            if (_outConstructor == null)
            {
                throw new Exception($"Default constructor for {_outType.Name} not found");
            }
        }

        public TOut Map(object source)
        {
            var sourceType = source.GetType();
            if (_converters.TryGetValue(sourceType, out var existsConverter))
            {
                return existsConverter(source);
            }

            var converter = BuildConverter(sourceType);
            _converters.Add(sourceType, converter);

            return converter(source);
        }

        public void PrepareForSource<TSource>()
        {
            var sourceType = typeof(TSource);
            
            if (!_converters.ContainsKey(sourceType)) return;

            var converter = BuildConverter(sourceType);
            _converters.Add(sourceType, converter);
        }
        
        private Func<object, TOut> BuildConverter(Type sourceType)
        {
            var parameter = Expression.Parameter(typeof(object), "source");
            
            var sourceInstance = Expression.Variable(sourceType, "typedSource");
            var outInstance = Expression.Variable(_outType, "outInstance");

            var expressions = new List<Expression>
            {
                Expression.Assign(sourceInstance, Expression.Convert(parameter, sourceType)),
                Expression.Assign(outInstance, Expression.New(_outConstructor))
            };
            
            var sourceProperties = sourceType.GetProperties();
            for (var i = 0; i < sourceProperties.Length; i++)
            {
                var sourceProperty = sourceProperties[i];

                if (_outProperties.TryGetValue(sourceProperty.Name, out var outProperty))
                {
                    var sourceValue = Expression.Property(sourceInstance, sourceProperty);
                    var outValue = Expression.Property(outInstance, outProperty);
                    
                    expressions.Add(Expression.Assign(outValue, sourceValue));
                }
            }

            expressions.Add(outInstance); // return
            
            var body = Expression.Block(new[] {sourceInstance, outInstance}, expressions);
            return Expression.Lambda<Func<object, TOut>>(body, parameter).Compile();
        }
    }
}