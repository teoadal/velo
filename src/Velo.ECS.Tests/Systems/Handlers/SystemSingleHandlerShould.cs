using System.Threading;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Tests.Systems.Handlers
{
    public class SystemSingleHandlerShould : ECSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Mock<IBeforeUpdateSystem> _system;

        private readonly SystemSingleHandler<IBeforeUpdateSystem> _handler;

        public SystemSingleHandlerShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _system = new Mock<IBeforeUpdateSystem>();

            _handler = new SystemSingleHandler<IBeforeUpdateSystem>(
                _system.Object,
                (system, ct) => system.BeforeUpdate(ct));
        }

        [Fact]
        public void Execute()
        {
            _handler
                .Awaiting(handler => handler.Execute(_ct))
                .Should().NotThrow();

            _system.Verify(s => s.BeforeUpdate(_ct));
        }
    }
}