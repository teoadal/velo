using System.Collections;
using System.Collections.Generic;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json
{
    internal abstract class JsonSource<TEntity> : ISource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IJsonEntityReader<TEntity> _reader;

        protected JsonSource(IJsonEntityReader<TEntity> reader)
        {
            _reader = reader;
        }

        public IEnumerable<TEntity> GetEntities(ISourceContext<TEntity> context)
        {
            var tokenizer = new JsonTokenizer(GetReader());
            return new Enumerator(_reader, tokenizer);
        }

        protected abstract JsonReader GetReader();

        private sealed class Enumerator : IEnumerable<TEntity>, IEnumerator<TEntity>
        {
            public TEntity Current { get; private set; }

            private readonly IJsonEntityReader<TEntity> _reader;
            private JsonTokenizer _tokenizer;

            public Enumerator(IJsonEntityReader<TEntity> reader, JsonTokenizer tokenizer)
            {
                _reader = reader;
                _tokenizer = tokenizer;

                Current = null!;
            }

            public IEnumerator<TEntity> GetEnumerator() => this;

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

                    var jsonObject = JsonVisitor.VisitObject(_tokenizer);
                    Current = _reader.Read(jsonObject);

                    return true;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
            }

            IEnumerator IEnumerable.GetEnumerator() => this;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _tokenizer.Dispose();
                _tokenizer = null!;

                Current = null!;
            }
        }
    }
}