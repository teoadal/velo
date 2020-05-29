using System.Collections.Generic;
using Velo.ECS.Sources.Context;
using Velo.Serialization;
using Velo.Serialization.Tokenization;

namespace Velo.ECS.Sources.Json
{
    internal abstract partial class JsonSource<TEntity> : IEntitySource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IConvertersCollection _converters;
        private readonly SourceDescriptions _descriptions;

        protected JsonSource(IConvertersCollection converters, SourceDescriptions descriptions)
        {
            _converters = converters;
            _descriptions = descriptions;
        }

        public IEnumerable<TEntity> GetEntities(IEntitySourceContext<TEntity> context)
        {
            var tokenizer = new JsonTokenizer(GetReader());

            tokenizer.Skip(JsonTokenType.ArrayStart);

            return new Enumerator(_converters, _descriptions, tokenizer);
        }

        protected abstract JsonReader GetReader();

        public virtual void Dispose()
        {
        }
    }
}