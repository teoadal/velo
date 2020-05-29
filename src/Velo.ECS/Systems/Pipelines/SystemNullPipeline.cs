using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;

namespace Velo.ECS.Systems.Pipelines
{
    internal sealed class SystemNullPipeline<TSystem> : ISystemPipeline<TSystem>
        where TSystem : class
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            return TaskUtils.CompletedTask;
        }
    }
}