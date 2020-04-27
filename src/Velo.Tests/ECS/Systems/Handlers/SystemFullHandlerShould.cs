using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemFullHandlerShould : ECSTestClass
    {
        private readonly CancellationToken _ct;

        private readonly SystemFullHandler<IUpdateSystem> _handler;
        private readonly Mock<ParallelSystem>[] _parallelSystems;
        private readonly Mock<IUpdateSystem>[] _sequentialSystems;

        public SystemFullHandlerShould(ITestOutputHelper output) : base(output)
        {
            _parallelSystems = Many(() => new Mock<ParallelSystem>());
            _sequentialSystems = Many(() => new Mock<IUpdateSystem>());

            _ct = CancellationToken.None;
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
                .Awaiting(handler => handler.Execute(_ct))
                .Should().NotThrow();

            foreach (var system in _parallelSystems)
            {
                system.Verify(s => s.Update(_ct), Times.Once);
            }

            foreach (var system in _sequentialSystems)
            {
                system.Verify(s => s.Update(_ct), Times.Once);
            }
        }
    }
}