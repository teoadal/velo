using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemSequentialHandlerShould : ECSTestClass
    {
        private readonly Mock<IInitSystem>[] _systems;

        private readonly SystemSequentialHandler<IInitSystem> _handler;

        public SystemSequentialHandlerShould()
        {
            _systems = Many(() => new Mock<IInitSystem>());

            _handler = new SystemSequentialHandler<IInitSystem>(
                _systems.Select(s => s.Object).ToArray(),
                (system, ct) => system.Init(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Init(CancellationToken));
            }
        }
    }
}