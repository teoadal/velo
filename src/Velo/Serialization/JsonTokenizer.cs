using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Velo.Serialization
{
    internal sealed class JsonTokenizer : IEnumerator<JToken>
    {
        public JToken Current { get; private set; }

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
                        Current = Read(JTokenType.ArrayStart);
                        return true;
                    case ']':
                        Current = Read(JTokenType.ArrayEnd);
                        return true;
                    case '{':
                        Current = Read(JTokenType.ObjectStart);
                        return true;
                    case '}':
                        Current = Read(JTokenType.ObjectEnd);
                        return true;
                    case '"':
                        Current = MaybeProperty(ReadString());
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

        private JToken MaybeProperty(string stringToken)
        {
            var isProperty = _serialized[_position] == ':';

            if (!isProperty) return new JToken(JTokenType.String, stringToken);

            SkipChar();
            return new JToken(JTokenType.Property, stringToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken Read(JTokenType tokenType)
        {
            SkipChar();
            return new JToken(tokenType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ReadFalse()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JToken(JTokenType.False);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ReadTrue()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JToken(JTokenType.True);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ReadNull()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JToken(JTokenType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JToken ReadNumber()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (!char.IsDigit(ch) && ch != '.') break;

                _builder.Append(ch);
            }

            var result = new JToken(JTokenType.Number, _builder.ToString());
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