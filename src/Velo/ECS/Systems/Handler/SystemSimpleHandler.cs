using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Handler
{
    internal sealed class SystemSimpleHandler<TSystem> : ISystemHandler<TSystem>
        where TSystem : class
    {
        private readonly TSystem _system;
        private readonly Func<TSystem, CancellationToken, Task> _update;

        public SystemSimpleHandler(TSystem system, Func<TSystem, CancellationToken, Task> update)
        {
            _system = system;
            _update = update;
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _update(_system, cancellationToken);
        }
    }
}