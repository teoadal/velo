using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IUpdateSystem
    {
        Task Update(CancellationToken cancellationToken);
    }
}