using System.Collections.Generic;
using System.Linq;
using Velo.Utils;

namespace Velo.ECS.Assets.Sources
{
    public interface IAssetSourceContext
    {
        Asset Get(int id);
    }

    internal sealed class AssetSourceContext : IAssetSourceContext
    {
        private readonly Dictionary<int, Asset> _assets;
        private readonly IAssetSource[] _sources;

        private IEnumerator<Asset>[] _enumerators;

        public AssetSourceContext(IAssetSource[] sources)
        {
            _assets = new Dictionary<int, Asset>(256);
            _enumerators = null!;

            _sources = sources;
        }

        public Asset Get(int id)
        {
            if (_assets.TryGetValue(id, out var exists)) return exists;

            foreach (var enumerator in _enumerators)
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    _assets.Add(current.Id, current);

                    if (current.Id == id) return current;
                }
            }

            throw Error.NotFound($"Asset with id '{id}' not found in sources");
        }

        public IEnumerable<Asset> GetAssets()
        {
            _enumerators = _sources
                .Select(source => source
                    .GetAssets(this)
                    .GetEnumerator())
                .ToArray();

            foreach (var enumerator in _enumerators)
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    _assets.Add(current.Id, current);
                }
            }

            foreach (var enumerator in _enumerators)
            {
                enumerator.Dispose();
            }

            return _assets.Values;
        }
    }
}