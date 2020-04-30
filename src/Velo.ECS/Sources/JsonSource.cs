using System.Collections.Generic;
using System.IO;
using System.Text;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    internal abstract class JsonSource<TEntity>
        where TEntity : class
    {
        protected IEnumerable<TEntity> Visit(Stream stream)
        {
            using var jsonReader = new JsonReader(stream);
            using var tokenizer = new JsonTokenizer(jsonReader, new StringBuilder(200));

            return VisitArray(tokenizer);
        }

        protected abstract TEntity VisitEntity(JsonTokenizer tokenizer);

        private IEnumerable<TEntity> VisitArray(JsonTokenizer tokenizer)
        {
            tokenizer.MoveNext(); // skip array start

            var entities = new List<TEntity>();
            while (tokenizer.MoveNext())
            {
                var tokenType = tokenizer.Current.TokenType;
                if (tokenType == JsonTokenType.ArrayEnd) break;
                if (tokenType != JsonTokenType.ObjectStart)
                {
                    throw Error.Deserialization(JsonTokenType.ObjectStart, tokenType);
                }

                var entity = VisitEntity(tokenizer);
                entities.Add(entity);
            }

            return entities;
        }
    }
}