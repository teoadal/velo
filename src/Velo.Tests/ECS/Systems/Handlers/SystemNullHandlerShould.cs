using System.Threading;
using FluentAssertions;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.Threading;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemNullHandlerShould : ECSTestClass
    {
        private readonly SystemNullHandler<ICleanupSystem> _handler;
        
        public SystemNullHandlerShould()
        {
            _handler = new SystemNullHandler<ICleanupSystem>();
        }

        [Fact]
        public void NothingExecute()
        {
            var task = _handler.Execute(CancellationToken.None);
            task.Should().Be(TaskUtils.CompletedTask);
        }
    }
}