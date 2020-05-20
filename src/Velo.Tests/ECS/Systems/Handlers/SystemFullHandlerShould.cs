using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemFullHandlerShould : ECSTestClass
    {
        private readonly SystemFullHandler<IUpdateSystem> _handler;
        private readonly Mock<ParallelSystem>[] _parallelSystems;
        private readonly Mock<IUpdateSystem>[] _sequentialSystems;

        public SystemFullHandlerShould()
        {
            _parallelSystems = Many(() => new Mock<ParallelSystem>());
            _sequentialSystems = Many(() => new Mock<IUpdateSystem>());
            
            var systems = _parallelSystems
                .Select(s => s.Object)
                .Concat(_sequentialSystems.Select(s => s.Object))
                .ToArray();

            _handler = new SystemFullHandler<IUpdateSystem>(
                systems,
                (system, ct) => system.Update(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            foreach (var system in _parallelSystems)
            {
                system.Verify(s => s.Update(CancellationToken), Times.Once);
            }

            foreach (var system in _sequentialSystems)
            {
                system.Verify(s => s.Update(CancellationToken), Times.Once);
            }
        }
    }
}