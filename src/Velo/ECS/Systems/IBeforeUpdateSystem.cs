using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IBeforeUpdateSystem : ISystem
    {
        ValueTask BeforeUpdate(CancellationToken cancellationToken);
    }
}