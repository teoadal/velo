using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IUpdateSystem : ISystem
    {
        Task Update(CancellationToken cancellationToken);
    }
}