using System.IO;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.ECS.Sources.Json
{
    internal sealed class JsonFileSource<TEntity> : JsonSource<TEntity>
        where TEntity: class, IEntity
    {
        private readonly string _path;

        public JsonFileSource(IJsonEntityReader<TEntity> reader, string path) : base(reader)
        {
            _path = path;
        }

        protected override JsonReader GetReader()
        {
            var fileStream = File.OpenRead(_path);
            return new JsonReader(fileStream, Encoding.UTF8);
        }
    }
}