using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Handlers
{
    internal sealed class SystemSequentialHandler<TSystem> : ISystemHandler<TSystem>
        where TSystem: class
    {
        private readonly TSystem[] _systems;
        private readonly Func<TSystem, CancellationToken, Task> _update;

        public SystemSequentialHandler(TSystem[] systems, Func<TSystem, CancellationToken, Task> update)
        {
            _systems = systems;
            _update = update;
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