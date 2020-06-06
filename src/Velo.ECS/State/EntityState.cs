using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Assets.Context;
using Velo.ECS.Components;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Json;
using Velo.ECS.Stores.Json;
using Velo.ECS.Systems;
using Velo.Serialization;

namespace Velo.ECS.State
{
    internal sealed class EntityState : IEntityState
    {
        public IActorContext Actors { get; }

        public IAssetContext Assets { get; }

        private readonly IActorFactory _actorFactory;
        private readonly IConvertersCollection _converters;
        private readonly SourceDescriptions _descriptions;

        public EntityState(
            IActorContext actors,
            IActorFactory actorFactory,
            IAssetContext assets,
            IConvertersCollection converters,
            SourceDescriptions descriptions)
        {
            Actors = actors;
            Assets = assets;

            _actorFactory = actorFactory;
            _converters = converters;
            _descriptions = descriptions;
        }

        #region Create

        public Actor Create(IComponent[]? components = null)
        {
            var actor = _actorFactory.Create(components);

            Actors.Add(actor);

            return actor;
        }

        public TActor Create<TActor>(IComponent[]? components = null) where TActor : Actor
        {
            var actor = _actorFactory.Create<TActor>(components);

            Actors.Add(actor);

            return actor;
        }

        #endregion

        public Task ClearAsync(ISystemService systems, CancellationToken cancellationToken = default)
        {
            Actors.Clear();

            return systems.CleanupAsync(cancellationToken);
        }

        public async Task LoadAsync(
            ISystemService systems,
            string jsonFilePath,
            CancellationToken cancellationToken = default)
        {
            await ClearAsync(systems, cancellationToken);

            using var source = new JsonFileSource<Actor>(_converters, _descriptions, jsonFilePath);

            Actors.Load(source);

            await systems.InitAsync(cancellationToken);
        }

        public async Task NewAsync(ISystemService systems, CancellationToken cancellationToken = default)
        {
            await ClearAsync(systems, cancellationToken);

            await systems.BootstrapAsync(cancellationToken);
            await systems.InitAsync(cancellationToken);
        }

        public void Save(string jsonFilePath)
        {
            var store = new JsonFileStore<Actor>(_converters, jsonFilePath);

            Actors.Save(store);
        }
    }
}