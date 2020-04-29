using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemSequentialHandlerShould : ECSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Mock<IInitSystem>[] _systems;

        private readonly SystemSequentialHandler<IInitSystem> _handler;

        public SystemSequentialHandlerShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _systems = Many(() => new Mock<IInitSystem>());

            _handler = new SystemSequentialHandler<IInitSystem>(
                _systems.Select(s => s.Object).ToArray(),
                (system, ct) => system.Init(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(_ct))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Init(_ct));
            }
        }
    }
}