using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.Serialization.Attributes;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Structs;
using Velo.Serialization.Tokenization;
using Velo.Text;
using Velo.Utils;

namespace Velo.Serialization
{
    internal static class SerializationUtils
    {
        public static Dictionary<string, IPropertyConverter<T>> ActivatePropertyConverters<T>(IServiceProvider services)
        {
            var ownerType = Typeof<T>.Raw;
            var properties = ownerType.GetProperties();

            var propertyConverters = new Dictionary<string, IPropertyConverter<T>>(
                properties.Length,
                StringUtils.IgnoreCaseComparer);

            var converters = (IConvertersCollection) services.GetService(typeof(IConvertersCollection));

            foreach (var property in properties)
            {
                if (IgnoreAttribute.IsDefined(property)) continue;

                IPropertyConverter<T> propertyConverter;
                if (ConverterAttribute.IsDefined(property))
                {
                    var converterType = property
                        .GetCustomAttribute<ConverterAttribute>()
                        .GetConverterType(ownerType, property);

                    var injections = new LocalList<object>(property, ownerType);
                    propertyConverter = (IPropertyConverter<T>) services.Activate(converterType, injections);
                }
                else
                {
                    propertyConverter = new PropertyConverter<T>(property, converters.Get(property.PropertyType));
                }

                propertyConverters.Add(property.Name, propertyConverter);
            }

            return propertyConverters;
        }

        public static Dictionary<string, StructPropertyConverter<T>> ActivateStructPropertyConverters<T>(
            IConvertersCollection converters)
            where T : struct
        {
            var ownerType = Typeof<T>.Raw;
            var properties = ownerType.GetProperties();

            var propertyConverters = new Dictionary<string, StructPropertyConverter<T>>(
                properties.Length,
                StringUtils.IgnoreCaseComparer);

            foreach (var property in properties)
            {
                if (IgnoreAttribute.IsDefined(property)) continue;

                var converter = converters.Get(property.PropertyType);
                var propertyConverter = new StructPropertyConverter<T>(ownerType, property, converter);

                propertyConverters.Add(property.Name, propertyConverter);
            }

            return propertyConverters;
        }

        public static TDelegate BuildDeserialize<TDelegate>(
            ParameterExpression instance,
            PropertyInfo property,
            Expression valueConverter) where TDelegate : Delegate
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var source = Expression.Parameter(typeof(JsonTokenizer), "source");

            var propertyValue = ExpressionUtils.Call(valueConverter, deserializeMethodName, source);
            var body = ExpressionUtils.SetProperty(instance, property, propertyValue);

            return Expression
                .Lambda<TDelegate>(body, source, instance)
                .Compile();
        }

        public static TDelegate BuildRead<TDelegate>(
            ParameterExpression instance,
            PropertyInfo property,
            Expression valueConverter) where TDelegate : Delegate
        {
            const string tryGetMethodName = nameof(JsonObject.TryGet);
            const string readMethodName = nameof(IJsonConverter<object>.Read);

            var source = Expression.Parameter(typeof(JsonObject), "source");

            var propertyName = Expression.Constant(property.Name);
            var propertyData = Expression.Variable(Typeof<JsonData>.Raw, "propertyData");
            var propertyExists = ExpressionUtils.Call(source, tryGetMethodName, propertyName, propertyData);

            var propertyValue = ExpressionUtils.Call(valueConverter, readMethodName, propertyData);
            var body = Expression.Block(
                new[] {propertyData},
                Expression.IfThen(propertyExists, ExpressionUtils.SetProperty(instance, property, propertyValue)));

            return Expression
                .Lambda<TDelegate>(body, source, instance)
                .Compile();
        }

        public static TDelegate BuildSerialize<TDelegate>(
            ParameterExpression instance,
            PropertyInfo property,
            Expression valueConverter) where TDelegate : Delegate
        {
            const string serializeMethodName = nameof(IJsonConverter<object>.Serialize);

            var output = Expression.Parameter(typeof(TextWriter), "output");

            var propertyValue = Expression.Property(instance, property);
            var body = ExpressionUtils.Call(valueConverter, serializeMethodName, propertyValue, output);

            return Expression
                .Lambda<TDelegate>(body, instance, output)
                .Compile();
        }

        public static TDelegate BuildWrite<TDelegate>(
            ParameterExpression instance,
            PropertyInfo property,
            Expression valueConverter) where TDelegate : Delegate
        {
            const string addMethodMane = nameof(JsonObject.Add);
            const string writeMethodName = nameof(IJsonConverter<object>.Write);

            var output = Expression.Parameter(typeof(JsonObject), "output");

            var propertyName = Expression.Constant(property.Name);
            var propertyValue = Expression.Property(instance, property);
            var propertyData = ExpressionUtils.Call(valueConverter, writeMethodName, propertyValue);

            var body = ExpressionUtils.Call(output, addMethodMane, propertyName, propertyData);

            return Expression
                .Lambda<TDelegate>(body, instance, output)
                .Compile();
        }
    }
}