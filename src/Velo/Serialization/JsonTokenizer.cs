using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Velo.Serialization
{
    internal sealed class JsonTokenizer : IEnumerator<JsonToken>
    {
        public const string FalseValue = "false";
        public const string NullValue = "null";
        public const string TrueValue = "true";

        public JsonToken Current { get; private set; }

        private StringBuilder _builder;
        private int _position;
        private string _serialized;

        public JsonTokenizer(string serialized, StringBuilder stringBuilder = null)
        {
            _serialized = serialized;
            _builder = stringBuilder ?? new StringBuilder();
        }

        public bool MoveNext()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];

                if (ch == ',' || ch == ' ')
                {
                    continue;
                }

                if (char.IsDigit(ch) || ch == '-')
                {
                    Current = ReadNumber();
                    return true;
                }

                switch (ch)
                {
                    case '[':
                        Current = Read(JsonTokenType.ArrayStart);
                        return true;
                    case ']':
                        Current = Read(JsonTokenType.ArrayEnd);
                        return true;
                    case '{':
                        Current = Read(JsonTokenType.ObjectStart);
                        return true;
                    case '}':
                        Current = Read(JsonTokenType.ObjectEnd);
                        return true;
                    case '"':
                        var stringValue = ReadString();
                        Current = MaybeProperty(stringValue);
                        return true;
                    case 'n':
                        Current = ReadNull();
                        return true;
                    case 'f':
                        Current = ReadFalse();
                        return true;
                    case 't':
                        Current = ReadTrue();
                        return true;
                }
            }

            return false;
        }

        public JsonTokenizer GetEnumerator() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken MaybeProperty(string stringToken)
        {
            var isProperty = _serialized.Length != _position && _serialized[_position] == ':';

            if (!isProperty)
            {
                return new JsonToken(JsonTokenType.String, stringToken);
            }

            SkipChar();
            return new JsonToken(JsonTokenType.Property, stringToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken Read(JsonTokenType tokenType)
        {
            SkipChar();
            return new JsonToken(tokenType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadFalse()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.False);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadTrue()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.True);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNull()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNumber()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (!char.IsDigit(ch) && ch != '.') break;

                _builder.Append(ch);
            }

            var result = new JsonToken(JsonTokenType.Number, _builder.ToString());
            _builder.Clear();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ReadString()
        {
            SkipChar();

            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];

                if (ch == '"')
                {
                    SkipChar();
                    break;
                }

                if (ch == '\\')
                {
                    SkipChar();
                    ch = serialized[_position];
                }

                _builder.Append(ch);
            }

            var result = _builder.ToString();
            _builder.Clear();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipChar()
        {
            _position++;
        }

        object IEnumerator.Current => Current;

        void IEnumerator.Reset()
        {
        }

        public void Dispose()
        {
            _builder = null;
            _position = -1;
            _serialized = null;
        }
    }
}