using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IBeforeUpdateSystem
    {
        Task BeforeUpdate(CancellationToken cancellationToken);
    }
}