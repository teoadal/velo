using System;
using System.Collections.Generic;
using System.Globalization;
using Velo.Collections;
using Velo.Serialization.Converters;
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
        private readonly DangerousVector<Type, IJsonConverter> _customConverters;

        public ConvertersCollection(CultureInfo? culture = null)
            : base(DefaultConverters(culture ?? CultureInfo.InvariantCulture))
        {
            _converterBuilder = BuildConverter;
            _customConverters = new DangerousVector<Type, IJsonConverter>();
        }

        public IJsonConverter Get(Type type)
        {
            return GetOrAdd(type, _converterBuilder);
        }

        public IJsonConverter<T> Get<T>()
        {
            return (IJsonConverter<T>) GetOrAdd(Typeof<T>.Raw, _converterBuilder);
        }

        public IJsonConverter GetCustomConverter(Type customConverterType)
        {
            return _customConverters.GetOrAdd(customConverterType,
                type => (IJsonConverter) Activator.CreateInstance(type));
        }

        private IJsonConverter BuildConverter(Type type)
        {
            if (type.IsArray)
            {
                return BuildArrayConverter(type);
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

            if (IsGenericImplementation(type, ArrayLikeGenericTypes))
            {
                return BuildArrayLikeConverter(type);
            }

            return IsGenericImplementation(type, ListGenericTypes)
                ? BuildListConverter(type)
                : BuildObjectConverter(type);
        }

        private IJsonConverter BuildArrayConverter(Type type)
        {
            var elementType = ReflectionUtils.GetArrayElementType(type);
            var elementConverter = Get(elementType);

            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private IJsonConverter BuildArrayLikeConverter(Type type)
        {
            var elementType = type.GenericTypeArguments[0];
            var elementConverter = Get(elementType);

            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private static IJsonConverter BuildEnumConverter(Type type)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType);
        }

        private IJsonConverter BuildListConverter(Type type)
        {
            var elementType = type.GenericTypeArguments[0];
            var elementConverter = Get(elementType);

            var converterType = typeof(ListConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private IJsonConverter BuildNullableConverter(Type underlyingType)
        {
            var valueConverter = Get(underlyingType);
            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
            return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
        }

        private IJsonConverter BuildObjectConverter(Type type)
        {
            var converterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, this);
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

        private static readonly Type[] ArrayLikeGenericTypes =
        {
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IReadOnlyCollection<>),
        };

        private static readonly Type[] ListGenericTypes =
        {
            typeof(List<>),
            typeof(IList<>)
        };

        private static bool IsGenericImplementation(Type type, Type[] genericTypes)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var generic in genericTypes)
            {
                if (ReflectionUtils.IsGenericTypeImplementation(type, generic))
                {
                    return true;
                }
            }

            return false;
        }
    }
}