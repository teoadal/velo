using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Handler
{
    internal sealed class SystemParallelHandler<TSystem> : ISystemHandler<TSystem>
        where TSystem: class
    {
        private readonly Task[] _buffer;

        private readonly TSystem[] _systems;
        private readonly Func<TSystem, CancellationToken, Task> _update;

        public SystemParallelHandler(TSystem[] systems, Func<TSystem, CancellationToken, Task> update)
        {
            _systems = systems;
            _update = update;

            _buffer = new Task[systems.Length];
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (var i = _buffer.Length - 1; i >= 0; i--)
            {
                _buffer[i] = _update(_systems[i], cancellationToken);
            }

            return Task.WhenAll(_buffer);
        }
    }
}