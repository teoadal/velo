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
            var tokenizer = new JsonTokenizer(jsonReader, new StringBuilder(200));

            var result = VisitArray(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }

        protected abstract TEntity VisitEntity(ref JsonTokenizer tokenizer);

        private IEnumerable<TEntity> VisitArray(ref JsonTokenizer tokenizer)
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

                var entity = VisitEntity(ref tokenizer);
                entities.Add(entity);
            }

            return entities;
        }
    }
}