using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Systems;
using Velo.Threading;

namespace Velo.TestsModels.ECS
{
    [Parallel]
    public abstract class ParallelSystem : IUpdateSystem
    {
        public abstract Task Update(CancellationToken cancellationToken);
    }
}