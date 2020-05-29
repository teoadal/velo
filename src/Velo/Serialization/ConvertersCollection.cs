using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Compatibility;
using Velo.Serialization.Attributes;
using Velo.Serialization.Collections;
using Velo.Serialization.Objects;
using Velo.Serialization.Primitives;
using Velo.Serialization.Structs;
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
        private readonly IServiceProvider _services;

        public ConvertersCollection(IServiceProvider services, CultureInfo? culture = null)
            : base(DefaultConverters(culture ?? CultureInfo.InvariantCulture))
        {
            _converterBuilder = BuildConverter;
            _services = services;

            var converters = services.GetArray<IJsonConverter>();
            if (converters == null) return;
            foreach (var converter in converters)
            {
                TryAdd(converter.Contract, converter);
            }
        }

        public ConvertersCollection(Func<Type, object> serviceProvider, CultureInfo? culture = null)
            : this(new DelegateServiceResolver(serviceProvider), culture)
        {
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
            if (TryGetCustomConverter(type, out var customConverter)) return customConverter;
            
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

            if (type.IsEnum)
            {
                return BuildEnumConverter(type);
            }
            
            return type.IsValueType
                ? BuildStructConverter(type)
                : BuildObjectConverter(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IJsonConverter BuildArrayConverter(Type elementType)
        {
            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IJsonConverter BuildEnumConverter(Type type)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IJsonConverter BuildListConverter(Type elementType)
        {
            var converterType = typeof(ListConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IJsonConverter BuildNullableConverter(Type underlyingType)
        {
            var valueConverter = Get(underlyingType);
            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
            return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IJsonConverter BuildObjectConverter(Type type)
        {
            var converterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, _services);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IJsonConverter BuildStructConverter(Type type)
        {
            var converterType = typeof(StructConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
        }

        private bool TryGetCustomConverter(Type type, out IJsonConverter customConverter)
        {
            if (!ConverterAttribute.IsDefined(type))
            {
                customConverter = null!;
                return false;
            }

            var customConverterType = type
                .GetCustomAttribute<ConverterAttribute>()
                .GetConverterType(type);

            var injections = new LocalList<object>(type, this);
            customConverter = (IJsonConverter) _services.Activate(customConverterType, injections);
            return true;
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
                {typeof(long), new LongConverter()},
                {typeof(string), new StringConverter()},
                {typeof(TimeSpan), new TimeSpanConverter(culture)}
            };
        }
    }
}