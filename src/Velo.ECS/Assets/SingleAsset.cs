using System.Diagnostics;
using Velo.Utils;

namespace Velo.ECS.Assets
{
    [DebuggerDisplay("{_instance.GetType().Name} {_instance.Id}")]
    public sealed class SingleAsset<TAsset>
        where TAsset : Asset
    {
        private readonly TAsset _instance = null!;

        public SingleAsset(Asset[] assets)
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