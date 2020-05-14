using System.IO;
using Velo.Serialization;
using Velo.Serialization.Tokenization;

namespace Velo.ECS.Sources.Json
{
    internal sealed class JsonStreamSource<TEntity> : JsonSource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly Stream _stream;

        public JsonStreamSource(IConvertersCollection converters, Stream stream) : base(converters)
        {
            _stream = stream;
        }

        protected override JsonReader GetReader()
        {
            return new JsonReader(_stream);
        }

        public override void Dispose()
        {
            _stream.Dispose();
            base.Dispose();
        }
    }
}