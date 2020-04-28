using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Tests.Systems.Handlers
{
    public class SystemParallelHandlerShould : ECSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Mock<ParallelSystem>[] _systems;
        
        private readonly SystemParallelHandler<IUpdateSystem> _handler;
        
        public SystemParallelHandlerShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _systems = Many(() => new Mock<ParallelSystem>());
            
            _handler = new SystemParallelHandler<IUpdateSystem>(
                _systems.Select(system => (IUpdateSystem)system.Object).ToArray(),
                (system, ct) => system.Update(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(_ct))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Update(_ct));
            }
        }
    }
}