using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface ISystemService
    {
        Task Cleanup(CancellationToken cancellationToken);

        Task Init(CancellationToken cancellationToken);

        Task Update(CancellationToken cancellationToken);
    }
}