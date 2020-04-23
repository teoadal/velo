using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Handler
{
    // ReSharper disable once UnusedTypeParameter
    internal interface ISystemHandler<TSystem> where TSystem: class
    {
        Task Execute(CancellationToken cancellationToken);
    }
}