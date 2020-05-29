using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;

namespace Velo.ECS.Systems.Pipelines
{
    internal sealed class SystemFullPipeline<TSystem> : ISystemPipeline<TSystem>
        where TSystem : class
    {
        private readonly SystemParallelPipeline<TSystem> _parallelPipeline;
        private readonly SystemSequentialPipeline<TSystem> _sequentialPipeline;

        public SystemFullPipeline(TSystem[] systems)
        {
            var parallelSystems = new List<TSystem>();
            var sequenceSystems = new List<TSystem>();

            foreach (var system in systems)
            {
                if (ParallelAttribute.IsDefined(system.GetType())) parallelSystems.Add(system);
                else sequenceSystems.Add(system);
            }

            _parallelPipeline = new SystemParallelPipeline<TSystem>(parallelSystems.ToArray());
            _sequentialPipeline = new SystemSequentialPipeline<TSystem>(sequenceSystems.ToArray());
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            await _parallelPipeline.Execute(cancellationToken);
            await _sequentialPipeline.Execute(cancellationToken);
        }
    }
}