using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface ISystemService
    {
        Task BootstrapAsync(CancellationToken cancellationToken = default);
        
        Task CleanupAsync(CancellationToken cancellationToken = default);

        Task InitAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(CancellationToken cancellationToken = default);
    }
}