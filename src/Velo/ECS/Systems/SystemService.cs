using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Systems.Handler;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService : ISystemService
    {
        private readonly ISystemHandler<ICleanupSystem> _cleanup;
        private readonly ISystemHandler<IInitSystem> _init;

        private readonly ISystemHandler<IBeforeUpdateSystem> _beforeHandler;
        private readonly ISystemHandler<IUpdateSystem> _update;
        private readonly ISystemHandler<IAfterUpdateSystem> _afterHandler;

        public SystemService(
            ISystemHandler<ICleanupSystem> cleanup, 
            ISystemHandler<IInitSystem> init, 
            ISystemHandler<IBeforeUpdateSystem> beforeHandler, 
            ISystemHandler<IUpdateSystem> update, 
            ISystemHandler<IAfterUpdateSystem> afterHandler)
        {
            _cleanup = cleanup;
            _init = init;
            _beforeHandler = beforeHandler;
            _update = update;
            _afterHandler = afterHandler;
        }

        public Task Cleanup(CancellationToken cancellationToken)
        {
            return _cleanup.Execute(cancellationToken);
        }

        public Task Init(CancellationToken cancellationToken)
        {
            return _init.Execute(cancellationToken);
        }

        public async Task Update(CancellationToken cancellationToken)
        {
            await _beforeHandler.Execute(cancellationToken);
            await _update.Execute(cancellationToken);
            await _afterHandler.Execute(cancellationToken);
        }
    }
}