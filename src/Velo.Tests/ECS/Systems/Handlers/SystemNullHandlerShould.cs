using System.Threading;
using FluentAssertions;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Velo.Threading;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemNullHandlerShould : ECSTestClass
    {
        private readonly SystemNullPipeline<ICleanupSystem> _pipeline;
        
        public SystemNullHandlerShould()
        {
            _pipeline = new SystemNullPipeline<ICleanupSystem>();
        }

        [Fact]
        public void NothingExecute()
        {
            var task = _pipeline.Execute(CancellationToken.None);
            task.Should().Be(TaskUtils.CompletedTask);
        }
    }
}