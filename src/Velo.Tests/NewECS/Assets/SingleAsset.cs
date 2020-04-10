using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Tests.NewECS.Assets
{
    public sealed class SingleAsset<TAsset>
        where TAsset : Asset
    {
        private readonly TAsset _instance;

        public SingleAsset(IEnumerable<Asset> assets)
        {
            foreach (var asset in assets)
            {
                if (!(asset is TAsset single)) continue;

                _instance = single;
                break;
            }

            if (_instance == null)
            {
                throw Error.NotFound($"Single asset with type {ReflectionUtils.GetName<TAsset>()} not found");
            }
        }

        public TAsset GetInstance() => _instance;

        public static implicit operator TAsset(SingleAsset<TAsset> single)
        {
            return single._instance;
        }
    }
}