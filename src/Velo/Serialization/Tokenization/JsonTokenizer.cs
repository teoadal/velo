using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Collections;

namespace Velo.Serialization.Tokenization
{
    [DebuggerDisplay("Current: {Current.TokenType} {Current.Value}")]
    internal sealed class JsonTokenizer : IEnumerator<JsonToken>
    {
        public JsonToken Current { get; private set; }

        private StringBuilder _builder;
        private JsonReader _reader;

        private bool _disposed;

        public JsonTokenizer(JsonReader reader, StringBuilder stringBuilder)
        {
            _reader = reader;
            _builder = stringBuilder;
            _disposed = false;

            Current = default;
        }

        public bool MoveNext()
        {
            if (!_reader.CanRead || _disposed) return false;

            do
            {
                var ch = _reader.Current;

                if (ch == ',' || ch == ' ' || ch == 0)
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
            } while (_reader.MoveNext());

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken MaybeProperty(string stringToken)
        {
            var isProperty = _reader.Current == ':';

            return isProperty
                ? new JsonToken(JsonTokenType.Property, stringToken)
                : new JsonToken(JsonTokenType.String, stringToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken Read(JsonTokenType tokenType)
        {
            var token = new JsonToken(tokenType);
            _reader.MoveNext();
            return token;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadFalse()
        {
            while (_reader.MoveNext())
                if (char.IsPunctuation(_reader.Current))
                    break;

            return new JsonToken(JsonTokenType.False);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadTrue()
        {
            while (_reader.MoveNext())
                if (char.IsPunctuation(_reader.Current))
                    break;

            return new JsonToken(JsonTokenType.True);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNull()
        {
            while (_reader.MoveNext())
                if (char.IsPunctuation(_reader.Current))
                    break;

            return new JsonToken(JsonTokenType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNumber()
        {
            var buffer = new LocalList<char>();

            do
            {
                var ch = _reader.Current;
                if (!char.IsDigit(ch) && ch != '.' && ch != '-') break;

                buffer.Add(ch);
            } while (_reader.MoveNext());

            var value = new string(buffer.ToArray());
            return new JsonToken(JsonTokenType.Number, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ReadString()
        {
            while (_reader.MoveNext())
            {
                var ch = _reader.Current;

                if (ch == '"')
                {
                    _reader.Skip();
                    break;
                }

                if (ch == '\\')
                {
                    _reader.Skip();
                    ch = _reader.Current;
                }

                _builder.Append(ch);
            }

            var result = _builder.ToString();
            _builder.Clear();
            return result;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (_disposed) return;

            Current = default;

            _builder = null!;
            
            _reader.Dispose();
            
            _disposed = true;
        }
    }
}