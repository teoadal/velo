using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;

namespace Velo.ECS.Systems.Handler
{
    internal sealed class SystemNullHandler<TSystem> : ISystemHandler<TSystem>
        where TSystem: class
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            return TaskUtils.CompletedTask;
        }
    }
}