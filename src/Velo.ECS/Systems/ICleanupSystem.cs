using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface ICleanupSystem
    {
        Task Cleanup(CancellationToken cancellationToken);
    }
}