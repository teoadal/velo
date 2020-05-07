using System.IO;
using System.Text;
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

        public JsonStreamSource(IConvertersCollection converters, string jsonData) : base(converters)
        {
            _stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
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