using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.Serialization.Attributes;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Text;
using Velo.Utils;

namespace Velo.Serialization
{
    internal static class SerializationExtensions
    {
        public static Dictionary<string, IPropertyConverter<T>> ActivatePropertyConverters<T>(
            this IServiceProvider services)
        {
            var ownerType = Typeof<T>.Raw;
            var properties = ownerType.GetProperties();

            var propertyConverters = new Dictionary<string, IPropertyConverter<T>>(
                properties.Length,
                StringUtils.IgnoreCaseComparer);

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
                    var converters = (IConvertersCollection) services.GetService(typeof(IConvertersCollection));
                    propertyConverter = new PropertyConverter<T>(property, converters.Get(property.PropertyType));
                }

                propertyConverters.Add(property.Name, propertyConverter);
            }

            return propertyConverters;
        }

        public static string GetNotNullPropertyName(this JsonToken token)
        {
            var tokenType = token.TokenType;

            if (tokenType != JsonTokenType.Property)
            {
                throw Error.Deserialization(JsonTokenType.Property, tokenType);
            }

            var propertyName = token.Value;

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw Error.Deserialization($"Expected not empty {JsonTokenType.Property} token");
            }

            return propertyName!;
        }

        public static T Deserialize<T>(this IJsonConverter<T> converter, string json, StringBuilder? sb = null)
        {
            using var tokenizer = new JsonTokenizer(json, sb ?? new StringBuilder());

            if (converter.IsPrimitive) tokenizer.MoveNext();
            return converter.Deserialize(tokenizer);
        }

        public static T Read<T>(this IConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }

        public static JsonData Write<T>(this IConvertersCollection converters, T data)
        {
            return converters.Get<T>().Write(data);
        }

        public static string Serialize(this JsonData data)
        {
            var stringWriter = new StringWriter();
            data.Serialize(stringWriter);
            return stringWriter.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TextWriter WriteProperty(this TextWriter writer, string propertyName)
        {
            writer.Write('"');
            writer.Write(propertyName);
            writer.Write("\":");

            return writer;
        }

        public static TextWriter WriteProperty(this TextWriter writer, string propertyName, JsonData propertyValue)
        {
            WriteProperty(writer, propertyName);

            propertyValue.Serialize(writer);

            return writer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TextWriter WriteString(this TextWriter writer, string value)
        {
            writer.Write('"');
            writer.Write(value);
            writer.Write('"');

            return writer;
        }
    }
}