using System.Threading;
using System.Threading.Tasks;
using Velo.ECS.Systems;

namespace Velo.TestsModels.ECS
{
    public sealed class ComplexSystem : IInitializeSystem, IBeforeUpdateSystem, IUpdateSystem, IAfterUpdateSystem
    {
        public int InitStep { get; private set; }
        public int BeforeStep { get; private set; }
        public int UpdateStep { get; private set; }
        public int AfterStep { get; private set; }
        
        private int _nextStep = 1;
        
        public Task Initialize(CancellationToken cancellationToken)
        {
            InitStep = _nextStep++;
            return Task.CompletedTask;
        }

        public Task BeforeUpdate(CancellationToken cancellationToken)
        {
            BeforeStep = _nextStep++;
            return Task.CompletedTask;
        }

        public Task Update(CancellationToken cancellationToken)
        {
            UpdateStep = _nextStep++;
            return Task.CompletedTask;
        }

        public Task AfterUpdate(CancellationToken cancellationToken)
        {
            AfterStep = _nextStep++;
            return Task.CompletedTask;
        }
    }
}