using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Assets.Context;
using Velo.ECS.Components;
using Velo.ECS.Systems;

namespace Velo.ECS.State
{
    public interface IEntityState
    {
        IActorContext Actors { get; }

        IAssetContext Assets { get; }

        Actor Create(IComponent[]? components = null);

        TActor Create<TActor>(IComponent[]? components = null) where TActor : Actor;

        Task ClearAsync(ISystemService systems, CancellationToken cancellationToken = default);

        Task LoadAsync(ISystemService systems, string jsonFilePath, CancellationToken cancellationToken = default);

        Task NewAsync(ISystemService systems, CancellationToken cancellationToken = default);

        void Save(string jsonFilePath);
    }
}