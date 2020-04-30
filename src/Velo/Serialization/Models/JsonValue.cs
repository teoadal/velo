using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Velo.Serialization.Converters;

namespace Velo.Serialization.Models
{
    [DebuggerDisplay("{Type} {Value}")]
    public sealed class JsonValue : JsonData, IEquatable<JsonValue>
    {
        public const string FalseToken = "false";
        public const string NullToken = "null";
        public const string TrueToken = "true";
        
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public static readonly JsonValue True = new JsonValue(TrueToken, JsonDataType.True);

        public static readonly JsonValue False = new JsonValue(FalseToken, JsonDataType.False);

        public static readonly JsonValue Null = new JsonValue(NullToken, JsonDataType.Null);

        public static readonly JsonValue StringEmpty = new JsonValue(string.Empty, JsonDataType.String);

        public static readonly JsonValue Zero = new JsonValue("0", JsonDataType.Number);

        public static JsonValue Boolean(bool value) => value ? True : False;

        public static JsonValue DateTime(DateTime value, CultureInfo? cultureInfo = null)
        {
            return new JsonValue(
                value.ToString(DateTimeConverter.Pattern, cultureInfo ?? Invariant),
                JsonDataType.String);
        }

        public static JsonValue Number(int value) => new JsonValue(value.ToString(), JsonDataType.Number);

        public static JsonValue Number(float value, CultureInfo? cultureInfo = null)
        {
            return new JsonValue(
                value.ToString(FloatConverter.Pattern, cultureInfo ?? Invariant), 
                JsonDataType.Number);
        }

        public static JsonValue Number(double value, CultureInfo? cultureInfo = null)
        {
            return new JsonValue(
                value.ToString(DoubleConverter.Pattern, cultureInfo ?? Invariant), 
                JsonDataType.Number);
        }

        public static JsonValue String(string? value)
        {
            if (value == null) return Null;
            return value == string.Empty
                ? StringEmpty
                : new JsonValue(value, JsonDataType.String);
        }

        public static JsonValue TimeSpan(TimeSpan value, CultureInfo? cultureInfo = null)
        {
            return new JsonValue(
                value.ToString(TimeSpanConverter.Pattern, cultureInfo ?? Invariant), 
                JsonDataType.Number);
        }

        public readonly string Value;

        public JsonValue(string value, JsonDataType type) : base(type)
        {
            Value = value;
        }

        public bool Equals(JsonValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Type == other.Type && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is JsonValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public override void Serialize(TextWriter writer)
        {
            if (Type == JsonDataType.String)
            {
                writer.Write('\"');
                writer.Write(Value);
                writer.Write('\"');
            }
            else
            {
                writer.Write(Value);
            }
        }
    }
}