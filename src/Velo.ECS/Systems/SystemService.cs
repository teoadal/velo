using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Systems.Pipelines;

namespace Velo.ECS.Systems
{
    internal sealed class SystemService : ISystemService
    {
        private readonly ISystemPipeline<IBootstrapSystem> _bootstrap;
        private readonly ISystemPipeline<ICleanupSystem> _cleanup;
        private readonly ISystemPipeline<IInitSystem> _init;

        private readonly ISystemPipeline<IBeforeUpdateSystem> _beforePipeline;
        private readonly ISystemPipeline<IUpdateSystem> _update;
        private readonly ISystemPipeline<IAfterUpdateSystem> _afterPipeline;

        public SystemService(
            ISystemPipeline<IBootstrapSystem> bootstrap,
            ISystemPipeline<IInitSystem> init, 
            ISystemPipeline<IBeforeUpdateSystem> beforePipeline, 
            ISystemPipeline<IUpdateSystem> update, 
            ISystemPipeline<IAfterUpdateSystem> afterPipeline,
            ISystemPipeline<ICleanupSystem> cleanup) 
        {
            _bootstrap = bootstrap;
            _cleanup = cleanup;
            _init = init;
            _beforePipeline = beforePipeline;
            _update = update;
            _afterPipeline = afterPipeline;
        }

        public Task Bootstrap(CancellationToken cancellationToken)
        {
            return _bootstrap.Execute(cancellationToken);
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
            await _beforePipeline.Execute(cancellationToken);
            await _update.Execute(cancellationToken);
            await _afterPipeline.Execute(cancellationToken);
        }
    }
}