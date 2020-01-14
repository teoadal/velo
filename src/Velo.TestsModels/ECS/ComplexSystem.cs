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
        
        public ValueTask Initialize(CancellationToken cancellationToken)
        {
            InitStep = _nextStep++;
            return new ValueTask();
        }

        public ValueTask BeforeUpdate(CancellationToken cancellationToken)
        {
            BeforeStep = _nextStep++;
            return new ValueTask();
        }

        public ValueTask Update(CancellationToken cancellationToken)
        {
            UpdateStep = _nextStep++;
            return new ValueTask();
        }

        public ValueTask AfterUpdate(CancellationToken cancellationToken)
        {
            AfterStep = _nextStep++;
            return new ValueTask();
        }
    }
}