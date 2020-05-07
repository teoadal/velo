using System.Collections;
using System.Collections.Generic;
using Velo.ECS.Sources.Context;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json
{
    internal abstract class JsonSource<TEntity> : IEntitySource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IConvertersCollection _converters;

        protected JsonSource(IConvertersCollection converters)
        {
            _converters = converters;
        }

        public IEnumerable<TEntity> GetEntities(IEntitySourceContext<TEntity> context)
        {
            var tokenizer = new JsonTokenizer(GetReader());

            tokenizer.Skip(JsonTokenType.ArrayStart);

            return new Enumerator(_converters, tokenizer);
        }

        protected abstract JsonReader GetReader();

        private sealed class Enumerator : IEnumerable<TEntity>, IEnumerator<TEntity>
        {
            public TEntity Current { get; private set; }

            private readonly IConvertersCollection _converters;
            private JsonTokenizer _tokenizer;

            public Enumerator(
                IConvertersCollection converters,
                JsonTokenizer tokenizer)
            {
                _converters = converters;
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

                    var entityData = JsonVisitor.VisitObject(_tokenizer);
                    var entityType = entityData.TryGet("_type", out var typeData)
                        ? SourceDescriptions.GetEntityType(_converters.Read<string>(typeData))
                        : Typeof<TEntity>.Raw;

                    var entityConverter = _converters.Get(entityType);
                    var entity = (TEntity) entityConverter.ReadObject(entityData)!;

                    Current = entity!;

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

        public virtual void Dispose()
        {
        }
    }
}