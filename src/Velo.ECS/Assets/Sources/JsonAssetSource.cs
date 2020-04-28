using System.Collections.Generic;
using System.IO;
using Velo.ECS.Sources;
using Velo.Serialization.Tokenization;

namespace Velo.ECS.Assets.Sources
{
    internal sealed class JsonAssetSource : JsonSource<Asset>, IAssetSource
    {
        private readonly JsonEntityConverters _converters;
        private readonly string _path;

        public JsonAssetSource(JsonEntityConverters converters, string path)
        {
            _converters = converters;
            _path = path;
        }

        public IEnumerable<Asset> GetAssets()
        {
            using var fileStream = File.OpenRead(_path);
            return Visit(fileStream);
        }

        protected override Asset VisitEntity(ref JsonTokenizer tokenizer)
        {
            return _converters.DeserializeAsset(ref tokenizer);
        }
    }
}