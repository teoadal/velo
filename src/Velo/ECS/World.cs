using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Systems;

namespace Velo.ECS
{
    public sealed class World
    {
        public readonly ActorContext Actors;
        
        public readonly AssetContext Assets;

        private readonly SystemService _systemService;

        internal World(ActorContext actors, AssetContext assets, SystemService systemService)
        {
            Actors = actors;
            Assets = assets;
            
            _systemService = systemService;
        }

        public Task Init(CancellationToken cancellationToken = default)
        {
            return _systemService.Initialize(cancellationToken);
        }
        
        public Task Update(CancellationToken cancellationToken = default)
        {
            return _systemService.Update(cancellationToken);
        }
    }
}