using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IAfterUpdateSystem
    {
        Task AfterUpdate(CancellationToken cancellationToken);
    }
}