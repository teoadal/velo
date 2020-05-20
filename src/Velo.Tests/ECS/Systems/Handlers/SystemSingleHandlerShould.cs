using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemSingleHandlerShould : ECSTestClass
    {
        private readonly Mock<IBeforeUpdateSystem> _system;

        private readonly SystemSingleHandler<IBeforeUpdateSystem> _handler;

        public SystemSingleHandlerShould()
        {
            _system = new Mock<IBeforeUpdateSystem>();

            _handler = new SystemSingleHandler<IBeforeUpdateSystem>(
                _system.Object,
                (system, ct) => system.BeforeUpdate(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            _system.Verify(s => s.BeforeUpdate(CancellationToken));
        }
    }
}