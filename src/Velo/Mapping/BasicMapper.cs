using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Velo.Mapping
{
    internal sealed class BasicMapper<TOut> : IMapper<TOut>
    {
        private readonly Func<TOut> _activator;
        private readonly Dictionary<string, PropertyInfo> _outProperties;

        public BasicMapper()
        {
            var outType = typeof(TOut);

            _outProperties = outType.GetProperties().ToDictionary(p => p.Name);

            var outInstanceConstructor = outType.GetConstructor(Array.Empty<Type>());
            _activator = outInstanceConstructor == null
                ? throw new Exception($"Default constructor for {outType.Name} not found")
                : Expression.Lambda<Func<TOut>>(Expression.New(outInstanceConstructor)).Compile();
        }

        public TOut Map(object source)
        {
            var outInstance = _activator();

            var sourceProperties = source.GetType().GetProperties();
            for (var i = 0; i < sourceProperties.Length; i++)
            {
                var sourceProperty = sourceProperties[i];

                if (_outProperties.TryGetValue(sourceProperty.Name, out var outProperty))
                {
                    var sourceValue = sourceProperty.GetValue(source);
                    outProperty.SetValue(outInstance, sourceValue);
                }
            }

            return outInstance;
        }
    }
}