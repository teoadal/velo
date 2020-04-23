using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems
{
    public interface IInitSystem
    {
        Task Init(CancellationToken cancellationToken);
    }
}