using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemParallelHandlerShould : ECSTestClass
    {
        private readonly Mock<ParallelSystem>[] _systems;
        
        private readonly SystemParallelPipeline<IUpdateSystem> _pipeline;
        
        public SystemParallelHandlerShould()
        {
            _systems = Many(() => new Mock<ParallelSystem>());
            
            _pipeline = new SystemParallelPipeline<IUpdateSystem>(
                _systems.Select(system => (IUpdateSystem)system.Object).ToArray());
        }

        [Fact]
        public void Execute()
        {
            _pipeline
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Update(CancellationToken));
            }
        }
    }
}