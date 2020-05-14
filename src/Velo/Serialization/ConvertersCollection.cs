using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Velo.Collections;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.Serialization.Attributes;
using Velo.Serialization.Collections;
using Velo.Serialization.Objects;
using Velo.Serialization.Primitives;
using Velo.Utils;

namespace Velo.Serialization
{
    internal interface IConvertersCollection
    {
        IJsonConverter Get(Type type);

        IJsonConverter<T> Get<T>();
    }

    internal sealed class ConvertersCollection : DangerousVector<Type, IJsonConverter>, IConvertersCollection
    {
        private readonly Func<Type, IJsonConverter> _converterBuilder;
        private readonly IServiceProvider _serviceProvider;

        public ConvertersCollection(IServiceProvider serviceProvider, CultureInfo? culture = null)
            : base(DefaultConverters(culture ?? CultureInfo.InvariantCulture))
        {
            _converterBuilder = BuildConverter;
            _serviceProvider = serviceProvider;
        }

        public IJsonConverter Get(Type type)
        {
            return GetOrAdd(type, _converterBuilder);
        }

        public IJsonConverter<T> Get<T>()
        {
            return (IJsonConverter<T>) GetOrAdd(Typeof<T>.Raw, _converterBuilder);
        }

        private IJsonConverter BuildConverter(Type type)
        {
            if (ReflectionUtils.IsArrayLikeGenericType(type, out var elementType))
            {
                return BuildArrayConverter(elementType);
            }

            if (ReflectionUtils.IsListLikeGenericType(type, out elementType))
            {
                return BuildListConverter(elementType);
            }
            
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return BuildNullableConverter(underlyingType);
            }

            return type.IsEnum 
                ? BuildEnumConverter(type) 
                : BuildObjectConverter(type);
        }

        private IJsonConverter BuildArrayConverter(Type elementType)
        {
            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
        }

        private static IJsonConverter BuildEnumConverter(Type type)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType);
        }

        private IJsonConverter BuildListConverter(Type elementType)
        {
            var converterType = typeof(ListConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
        }

        private IJsonConverter BuildNullableConverter(Type underlyingType)
        {
            var valueConverter = Get(underlyingType);
            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
            return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
        }

        private IJsonConverter BuildObjectConverter(Type type)
        {
            if (ConverterAttribute.IsDefined(type))
            {
                var customConverterType = type
                    .GetCustomAttribute<ConverterAttribute>()
                    .GetConverterType(type);

                var injections = new LocalList<object>(type, this);
                return (IJsonConverter) _serviceProvider.Activate(customConverterType, injections);
            }

            var converterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, _serviceProvider);
        }

        private static Dictionary<Type, IJsonConverter> DefaultConverters(CultureInfo culture)
        {
            return new Dictionary<Type, IJsonConverter>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(DateTime), new DateTimeConverter(culture)},
                {typeof(decimal), new DecimalConverter(culture)},
                {typeof(double), new DoubleConverter(culture)},
                {typeof(float), new FloatConverter(culture)},
                {typeof(Guid), new GuidConverter()},
                {typeof(int), new IntConverter()},
                {typeof(string), new StringConverter()},
                {typeof(TimeSpan), new TimeSpanConverter(culture)}
            };
        }
    }
}