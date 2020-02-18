using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IInitializeSystem : ISystem
    {
        Task Initialize(CancellationToken cancellationToken);
    }
}