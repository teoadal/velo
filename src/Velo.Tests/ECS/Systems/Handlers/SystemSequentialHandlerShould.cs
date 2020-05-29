using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemSequentialHandlerShould : ECSTestClass
    {
        private readonly Mock<IInitSystem>[] _systems;

        private readonly SystemSequentialPipeline<IInitSystem> _pipeline;

        public SystemSequentialHandlerShould()
        {
            _systems = Many(() => new Mock<IInitSystem>());

            _pipeline = new SystemSequentialPipeline<IInitSystem>(
                _systems.Select(s => s.Object).ToArray());
        }

        [Fact]
        public void Execute()
        {
            _pipeline
                .Awaiting(handler => handler.Execute(CancellationToken))
                .Should().NotThrow();

            foreach (var system in _systems)
            {
                system.Verify(s => s.Init(CancellationToken));
            }
        }
    }
}