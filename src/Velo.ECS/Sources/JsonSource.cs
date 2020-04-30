using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    internal sealed class JsonSource<T> : IEnumerable<T>, IEnumerator<T>
        where T : class
    {
        public T Current { get; private set; }

        private Func<JsonTokenizer, T> _deserializer;

        private JsonTokenizer _tokenizer;

        public JsonSource(Stream stream, Func<JsonTokenizer, T> deserializer)
            : this(new JsonTokenizer(new JsonReader(stream), new StringBuilder(200)), deserializer)
        {
        }

        public JsonSource(JsonTokenizer tokenizer, Func<JsonTokenizer, T> deserializer)
        {
            _deserializer = deserializer;
            Current = null!;

            _tokenizer = tokenizer;

            _tokenizer.MoveNext(); // skip array start
        }

        public IEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            while (_tokenizer.MoveNext())
            {
                var tokenType = _tokenizer.Current.TokenType;
                if (tokenType == JsonTokenType.ArrayEnd) break;
                if (tokenType != JsonTokenType.ObjectStart)
                {
                    throw Error.Deserialization(JsonTokenType.ObjectStart, tokenType);
                }

                Current = _deserializer(_tokenizer);
                return true;
            }

            return false;
        }

        object IEnumerator.Current => Current;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IEnumerator.Reset()
        {
        }

        public void Dispose()
        {
            _deserializer = null!;

            _tokenizer?.Dispose();
            _tokenizer = null!;
        }
    }
}