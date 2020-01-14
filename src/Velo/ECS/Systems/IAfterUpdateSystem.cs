using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IAfterUpdateSystem : ISystem
    {
        ValueTask AfterUpdate(CancellationToken cancellationToken);
    }
}