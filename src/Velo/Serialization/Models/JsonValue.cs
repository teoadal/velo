using System;
using System.Globalization;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Models
{
    internal sealed class JsonValue : JsonData, IEquatable<JsonValue>
    {
        public static JsonValue Boolean(bool value) => value ? True : False;

        public static readonly JsonValue True = new JsonValue(JsonTokenizer.TokenTrueValue, JsonDataType.True);

        public static readonly JsonValue False = new JsonValue(JsonTokenizer.TokenFalseValue, JsonDataType.False);

        public static readonly JsonValue Null = new JsonValue(null, JsonDataType.Null);

        public static JsonValue Number(int value) => new JsonValue(value.ToString(), JsonDataType.Number);
        
        public static JsonValue Number(float value, CultureInfo cultureInfo = null)
        {
            return new JsonValue(value.ToString(cultureInfo ?? CultureInfo.InvariantCulture), JsonDataType.Number);
        }

        public static JsonValue String(string value) => string.IsNullOrWhiteSpace(value)
            ? StringEmpty
            : new JsonValue(value, JsonDataType.String);
        
        public static readonly JsonValue StringEmpty =  new JsonValue(string.Empty, JsonDataType.String);
        
        public static readonly JsonValue Zero = new JsonValue("0", JsonDataType.Number);

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
    }
}