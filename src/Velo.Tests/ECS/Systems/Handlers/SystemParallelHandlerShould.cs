using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemParallelHandlerShould : ECSTestClass
    {
        private readonly Mock<ParallelSystem>[] _systems;
        
        private readonly SystemParallelHandler<IUpdateSystem> _handler;
        
        public SystemParallelHandlerShould()
        {
            _systems = Many(() => new Mock<ParallelSystem>());
            
            _handler = new SystemParallelHandler<IUpdateSystem>(
                _systems.Select(system => (IUpdateSystem)system.Object).ToArray(),
                (system, ct) => system.Update(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Update(CancellationToken));
            }
        }
    }
}