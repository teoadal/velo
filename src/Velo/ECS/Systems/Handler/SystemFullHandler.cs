using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;

namespace Velo.ECS.Systems.Handler
{
    internal sealed class SystemFullHandler<TSystem> : ISystemHandler<TSystem>
        where TSystem: class
    {
        private readonly SystemParallelHandler<TSystem> _parallelHandler;
        private readonly SystemSequentialHandler<TSystem> _sequentialHandler;

        public SystemFullHandler(TSystem[] systems, Func<TSystem, CancellationToken, Task> update)
        {
            var parallelSystems = new List<TSystem>();
            var sequenceSystems = new List<TSystem>();

            foreach (var system in systems)
            {
                if (ParallelAttribute.IsDefined(system.GetType())) parallelSystems.Add(system);
                else sequenceSystems.Add(system);
            }

            _parallelHandler = new SystemParallelHandler<TSystem>(parallelSystems.ToArray(), update);
            _sequentialHandler = new SystemSequentialHandler<TSystem>(sequenceSystems.ToArray(), update);
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            await _parallelHandler.Execute(cancellationToken);
            await _sequentialHandler.Execute(cancellationToken);
        }
    }
}