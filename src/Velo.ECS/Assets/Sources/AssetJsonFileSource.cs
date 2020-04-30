using System.Collections.Generic;
using System.IO;
using Velo.ECS.Sources;

namespace Velo.ECS.Assets.Sources
{
    internal sealed class AssetJsonFileSource : IAssetSource
    {
        private readonly JsonEntityConverters _converters;
        private readonly string _path;

        public AssetJsonFileSource(JsonEntityConverters converters, string path)
        {
            _converters = converters;
            _path = path;
        }

        public IEnumerable<Asset> GetAssets(IAssetSourceContext sourceContext)
        {
            var fileStream = File.OpenRead(_path); // closed by stream reader
            return new JsonSource<Asset>(fileStream, tokenizer => _converters.DeserializeAsset(tokenizer));
        }
    }
}