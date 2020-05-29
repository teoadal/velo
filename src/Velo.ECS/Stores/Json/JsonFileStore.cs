using System.Collections.Generic;
using System.IO;
using System.Text;
using Velo.Serialization;

namespace Velo.ECS.Stores.Json
{
    internal sealed class JsonFileStore<TEntity> : IEntityStore<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IConvertersCollection _converters;
        private readonly string _path;

        public JsonFileStore(IConvertersCollection converters, string path)
        {
            _converters = converters;
            _path = path;
        }

        public void Write(IEnumerable<TEntity> actors)
        {
            using var writer = (TextWriter) new StreamWriter(_path, false, Encoding.UTF8);

            writer.WriteArrayStart();

            var first = true;
            foreach (var actor in actors)
            {
                if (first) first = false;
                else writer.Write(',');

                var converter = _converters.Get(actor.GetType());
                converter.SerializeObject(actor, writer);
            }

            writer.WriteArrayEnd();
        }
    }
}