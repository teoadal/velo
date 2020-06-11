using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;
using Velo.Utils;

namespace Velo.ECS.Systems.ActionSystems
{
    internal sealed class BootstrapActionSystem<TContext> : IBootstrapSystem, IDisposable
        where TContext : class
    {
        private TContext _context;
        private Action<TContext> _system;

        private bool _disposed;

        public BootstrapActionSystem(TContext context, Action<TContext> system)
        {
            _context = context;
            _system = system;
        }

        public Task Bootstrap(CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(BootstrapActionSystem<TContext>));

            _system(_context);

            return TaskUtils.CompletedTask;
        }

        public void Dispose()
        {
            _context = null!;
            _system = null!;
            _disposed = true;
        }
    }
}