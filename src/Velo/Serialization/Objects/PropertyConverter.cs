using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.Serialization.Attributes;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Text;
using Velo.Utils;

namespace Velo.Serialization.Objects
{
    internal sealed class PropertyConverter<TOwner> : IPropertyConverter<TOwner>
    {
        private readonly Action<JsonTokenizer, TOwner> _deserialize;
        private readonly Action<JsonObject, TOwner> _read;
        private readonly Action<TOwner, TextWriter> _serialize;
        private readonly Action<TOwner, JsonObject> _write;

        private PropertyConverter(PropertyInfo propertyInfo, IJsonConverter valueConverter)
        {
            var instance = Expression.Parameter(Typeof<TOwner>.Raw, "instance");
            var converter = Expression.Constant(valueConverter, valueConverter.GetType());

            _serialize = BuildSerializeMethod(instance, propertyInfo, converter);
            _write = BuildWriteMethod(instance, propertyInfo, converter);

            if (propertyInfo.CanWrite)
            {
                _deserialize = BuildDeserializeMethod(instance, propertyInfo, converter);
                _read = BuildReadMethod(instance, propertyInfo, converter);
            }
            else
            {
                _deserialize = ReadonlyProperty;
                _read = ReadonlyProperty;
            }
        }

        public void Deserialize(JsonTokenizer source, TOwner instance) => _deserialize(source, instance);

        public void Read(JsonObject source, TOwner instance) => _read(source, instance);

        public void Serialize(TOwner instance, TextWriter output) => _serialize(instance, output);

        public void Write(TOwner instance, JsonObject output) => _write(instance, output);

        private static Action<JsonTokenizer, TOwner> BuildDeserializeMethod(ParameterExpression instance,
            PropertyInfo property, Expression valueConverter)
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var source = Expression.Parameter(Typeof<JsonTokenizer>.Raw, "source");

            var propertyValue = ExpressionUtils.Call(valueConverter, deserializeMethodName, source);
            var body = ExpressionUtils.SetProperty(instance, property, propertyValue);

            return Expression
                .Lambda<Action<JsonTokenizer, TOwner>>(body, source, instance)
                .Compile();
        }

        private static Action<JsonObject, TOwner> BuildReadMethod(ParameterExpression instance, PropertyInfo property,
            Expression valueConverter)
        {
            const string tryGetMethodName = nameof(JsonObject.TryGet);
            const string readMethodName = nameof(IJsonConverter<object>.Read);

            var source = Expression.Parameter(Typeof<JsonObject>.Raw, "source");

            var propertyName = Expression.Constant(property.Name);
            var propertyData = Expression.Variable(Typeof<JsonData>.Raw, "propertyData");
            var propertyExists = ExpressionUtils.Call(source, tryGetMethodName, propertyName, propertyData);

            var propertyValue = ExpressionUtils.Call(valueConverter, readMethodName, propertyData);
            var body = Expression.Block(
                new[] {propertyData},
                Expression.IfThen(propertyExists, ExpressionUtils.SetProperty(instance, property, propertyValue)));

            return Expression
                .Lambda<Action<JsonObject, TOwner>>(body, source, instance)
                .Compile();
        }

        private static Action<TOwner, TextWriter> BuildSerializeMethod(ParameterExpression instance,
            PropertyInfo property, Expression valueConverter)
        {
            const string serializeMethodName = nameof(IJsonConverter<object>.Serialize);

            var output = Expression.Parameter(Typeof<TextWriter>.Raw, "output");

            var propertyValue = Expression.Property(instance, property);
            var body = ExpressionUtils.Call(valueConverter, serializeMethodName, propertyValue, output);

            return Expression
                .Lambda<Action<TOwner, TextWriter>>(body, instance, output)
                .Compile();
        }

        private static Action<TOwner, JsonObject> BuildWriteMethod(ParameterExpression instance, PropertyInfo property,
            Expression valueConverter)
        {
            const string addMethodMane = nameof(JsonObject.Add);
            const string writeMethodName = nameof(IJsonConverter<object>.Write);

            var output = Expression.Parameter(Typeof<JsonObject>.Raw, "output");

            var propertyName = Expression.Constant(property.Name);
            var propertyValue = Expression.Property(instance, property);
            var propertyData = ExpressionUtils.Call(valueConverter, writeMethodName, propertyValue);

            var body = ExpressionUtils.Call(output, addMethodMane, propertyName, propertyData);

            return Expression
                .Lambda<Action<TOwner, JsonObject>>(body, instance, output)
                .Compile();
        }

        private static void ReadonlyProperty(JsonTokenizer tokenizer, TOwner obj)
        {
        }

        private static void ReadonlyProperty(JsonData json, TOwner obj)
        {
        }

        public static Dictionary<string, IPropertyConverter<TOwner>> CreateCollection(
            IServiceProvider services,
            IConvertersCollection? converters = null)
        {
            var ownerType = Typeof<TOwner>.Raw;
            var properties = ownerType.GetProperties();

            var propertyConverters = new Dictionary<string, IPropertyConverter<TOwner>>(
                properties.Length,
                StringUtils.IgnoreCaseComparer);

            converters ??= (IConvertersCollection) services.GetService(typeof(IConvertersCollection));

            foreach (var property in properties)
            {
                if (IgnoreAttribute.IsDefined(property)) continue;

                IPropertyConverter<TOwner> propertyConverter;
                if (ConverterAttribute.IsDefined(property))
                {
                    var converterType = property
                        .GetCustomAttribute<ConverterAttribute>()
                        .GetConverterType(ownerType, property);

                    var injections = new LocalList<object>(property, ownerType);
                    propertyConverter = (IPropertyConverter<TOwner>) services.Activate(converterType, injections);
                }
                else
                {
                    var propertyValueConverter = converters.Get(property.PropertyType);
                    propertyConverter = new PropertyConverter<TOwner>(property, propertyValueConverter);
                }

                propertyConverters.Add(property.Name, propertyConverter);
            }

            return propertyConverters;
        }
    }
}