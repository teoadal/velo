using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal readonly struct PropertyConverter<TObject>
    {
        public readonly DeserializeMethod<TObject> Deserialize;
        public readonly Action<TObject, JsonData> Read;
        public readonly Action<TObject, TextWriter> Serialize;
        public readonly Func<TObject, JsonData> Write;

        public PropertyConverter(PropertyInfo propertyInfo, IJsonConverter valueConverter)
        {
            var instance = Expression.Parameter(Typeof<TObject>.Raw, "instance");
            var converter = Expression.Constant(valueConverter, valueConverter.GetType());

            Deserialize = BuildDeserializeMethod(instance, propertyInfo, converter);
            Read = BuildReadMethod(instance, propertyInfo, converter);
            Serialize = BuildSerializeMethod(instance, propertyInfo, converter);
            Write = BuildWriteMethod(instance, propertyInfo, converter);
        }

        private static DeserializeMethod<TObject> BuildDeserializeMethod(ParameterExpression instance,
            PropertyInfo property, Expression valueConverter)
        {
            const string deserializeMethodName = nameof(IJsonConverter<object>.Deserialize);

            var tokenizer = Expression.Parameter(JsonTokenizer.ByRefType, "tokenizer");

            var propertyValue = ExpressionUtils.Call(valueConverter, deserializeMethodName, tokenizer);
            var body = ExpressionUtils.SetProperty(instance, property, propertyValue);

            return Expression
                .Lambda<DeserializeMethod<TObject>>(body, instance, tokenizer)
                .Compile();
        }

        private static Action<TObject, JsonData> BuildReadMethod(ParameterExpression instance, PropertyInfo property,
            Expression valueConverter)
        {
            const string readMethodName = nameof(IJsonConverter<object>.Read);

            var data = Expression.Parameter(Typeof<JsonData>.Raw, "data");

            var propertyValue = ExpressionUtils.Call(valueConverter, readMethodName, data);
            var body = ExpressionUtils.SetProperty(instance, property, propertyValue);

            return Expression
                .Lambda<Action<TObject, JsonData>>(body, instance, data)
                .Compile();
        }

        private static Action<TObject, TextWriter> BuildSerializeMethod(ParameterExpression instance,
            PropertyInfo property, Expression valueConverter)
        {
            const string serializeMethodName = nameof(IJsonConverter<object>.Serialize);

            var writer = Expression.Parameter(Typeof<TextWriter>.Raw, "writer");

            var propertyValue = Expression.Property(instance, property);
            var body = ExpressionUtils.Call(valueConverter, serializeMethodName, propertyValue, writer);

            return Expression
                .Lambda<Action<TObject, TextWriter>>(body, instance, writer)
                .Compile();
        }

        private static Func<TObject, JsonData> BuildWriteMethod(ParameterExpression instance, PropertyInfo property,
            Expression valueConverter)
        {
            const string writeMethodName = nameof(IJsonConverter<object>.Write);

            var propertyValue = Expression.Property(instance, property);
            var body = ExpressionUtils.Call(valueConverter, writeMethodName, propertyValue);

            return Expression
                .Lambda<Func<TObject, JsonData>>(body, instance)
                .Compile();
        }
    }

    internal delegate void DeserializeMethod<in TObject>(TObject obj, ref JsonTokenizer tokenizer);
}