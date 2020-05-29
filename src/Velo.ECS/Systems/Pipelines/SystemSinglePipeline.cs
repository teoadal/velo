using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Pipelines
{
    internal sealed class SystemSinglePipeline<TSystem> : ISystemPipeline<TSystem>
        where TSystem : class
    {
        private readonly TSystem _system;
        private readonly Func<TSystem, CancellationToken, Task> _update;

        public SystemSinglePipeline(TSystem system)
        {
            _system = system;
            _update = ECSUtils.BuildSystemUpdateMethod<TSystem>();
        }

        public Task Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _update(_system, cancellationToken);
        }
    }
}