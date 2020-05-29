using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IBootstrapSystem
    {
        Task Bootstrap(CancellationToken cancellationToken);
    }
}