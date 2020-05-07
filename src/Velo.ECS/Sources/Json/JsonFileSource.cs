using System.IO;
using Velo.Serialization;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json
{
    internal sealed class JsonFileSource<TEntity> : JsonSource<TEntity>
        where TEntity : class, IEntity
    {
        private readonly string _path;

        public JsonFileSource(IConvertersCollection converters, string path) : base(converters)
        {
            if (!File.Exists(path)) throw Error.FileNotFound(path);

            _path = path;
        }

        protected override JsonReader GetReader()
        {
            var fileStream = File.OpenRead(_path);
            return new JsonReader(fileStream);
        }
    }
}