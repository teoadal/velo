using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemSingleHandlerShould : ECSTestClass
    {
        private readonly Mock<IBeforeUpdateSystem> _system;

        private readonly SystemSinglePipeline<IBeforeUpdateSystem> _pipeline;

        public SystemSingleHandlerShould()
        {
            _system = new Mock<IBeforeUpdateSystem>();

            _pipeline = new SystemSinglePipeline<IBeforeUpdateSystem>(_system.Object);
        }

        [Fact]
        public void Execute()
        {
            _pipeline
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            _system.Verify(s => s.BeforeUpdate(CancellationToken));
        }
    }
}