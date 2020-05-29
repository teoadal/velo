using System.Threading;
using System.Threading.Tasks;

namespace Velo.ECS.Systems.Pipelines
{
    // ReSharper disable once UnusedTypeParameter
    internal interface ISystemPipeline<TSystem> where TSystem : class
    {
        Task Execute(CancellationToken cancellationToken);
    }
}