using System.Collections;
using System.Collections.Generic;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json
{
    internal abstract partial class JsonSource<TEntity>
    {
                private sealed class Enumerator : IEnumerable<TEntity>, IEnumerator<TEntity>
        {
            public TEntity Current { get; private set; }

            private readonly IConvertersCollection _converters;
            private readonly SourceDescriptions _descriptions;
            private JsonTokenizer _tokenizer;

            public Enumerator(
                IConvertersCollection converters,
                SourceDescriptions descriptions,
                JsonTokenizer tokenizer)
            {
                _converters = converters;
                _descriptions = descriptions;
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
                        ? _descriptions.GetEntityType(_converters.Read<string>(typeData))
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

    }
}