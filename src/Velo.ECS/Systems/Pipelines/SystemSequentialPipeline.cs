using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Pipelines
{
    internal sealed class SystemSequentialPipeline<TSystem> : ISystemPipeline<TSystem>
        where TSystem : class
    {
        private readonly TSystem[] _systems;
        private readonly Func<TSystem, CancellationToken, Task> _update;

        public SystemSequentialPipeline(TSystem[] systems)
        {
            _systems = systems;
            _update = ECSUtils.BuildSystemUpdateMethod<TSystem>();
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var system in _systems)
            {
                await _update(system, cancellationToken);
            }
        }
    }
}