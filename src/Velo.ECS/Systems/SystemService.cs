using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Systems.Pipelines;
using Velo.Utils;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService : ISystemService, IDisposable
    {
        private ISystemPipeline<IBootstrapSystem> _bootstrap;
        private ISystemPipeline<ICleanupSystem> _cleanup;

        private ISystemPipeline<IInitSystem> _init;

        private ISystemPipeline<IBeforeUpdateSystem> _beforeUpdate;
        private ISystemPipeline<IUpdateSystem> _update;
        private ISystemPipeline<IAfterUpdateSystem> _afterUpdate;

        private bool _disposed;

        public SystemService(
            ISystemPipeline<IBootstrapSystem> bootstrap,
            ISystemPipeline<ICleanupSystem> cleanup,
            ISystemPipeline<IInitSystem> init,
            ISystemPipeline<IBeforeUpdateSystem> beforeUpdate,
            ISystemPipeline<IUpdateSystem> update,
            ISystemPipeline<IAfterUpdateSystem> afterUpdate)
        {
            _bootstrap = bootstrap;
            _cleanup = cleanup;

            _init = init;

            _beforeUpdate = beforeUpdate;
            _update = update;
            _afterUpdate = afterUpdate;
        }

        public Task BootstrapAsync(CancellationToken cancellationToken)
        {
            EnsureNotDisposed();

            return _bootstrap.Execute(cancellationToken);
        }

        public Task CleanupAsync(CancellationToken cancellationToken)
        {
            EnsureNotDisposed();

            return _cleanup.Execute(cancellationToken);
        }

        public Task InitAsync(CancellationToken cancellationToken)
        {
            EnsureNotDisposed();

            return _init.Execute(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            EnsureNotDisposed();

            await _beforeUpdate.Execute(cancellationToken);
            await _update.Execute(cancellationToken);
            await _afterUpdate.Execute(cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (!_disposed) return;

            throw Error.Disposed(nameof(SystemService));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _bootstrap = null!;
            _cleanup = null!;
            _init = null!;

            _beforeUpdate = null!;
            _update = null!;
            _afterUpdate = null!;

            _disposed = true;
        }
    }
}