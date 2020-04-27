using System.Threading;
using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Systems
{
    public class SystemServiceShould : ECSTestClass
    {
        private readonly CancellationToken _ct;

        private readonly Mock<ISystemHandler<IInitSystem>> _init;
        private readonly Mock<ISystemHandler<IBeforeUpdateSystem>> _beforeUpdate;
        private readonly Mock<ISystemHandler<IUpdateSystem>> _update;
        private readonly Mock<ISystemHandler<IAfterUpdateSystem>> _afterUpdate;
        private readonly Mock<ISystemHandler<ICleanupSystem>> _cleanup;

        private readonly ISystemService _systemService;

        public SystemServiceShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;

            _init = new Mock<ISystemHandler<IInitSystem>>();
            _beforeUpdate = new Mock<ISystemHandler<IBeforeUpdateSystem>>();
            _update = new Mock<ISystemHandler<IUpdateSystem>>();
            _afterUpdate = new Mock<ISystemHandler<IAfterUpdateSystem>>();
            _cleanup = new Mock<ISystemHandler<ICleanupSystem>>();

            _systemService = new SystemService(
                _init.Object,
                _beforeUpdate.Object,
                _update.Object,
                _afterUpdate.Object,
                _cleanup.Object);
        }

        [Fact]
        public void RunInitSystems()
        {
            _systemService
                .Awaiting(service => service.Init(_ct))
                .Should().NotThrow();
            
            _init.Verify(init => init.Execute(_ct));
        }

        [Fact]
        public void RunUpdateSystems()
        {
            _systemService
                .Awaiting(service => service.Update(_ct))
                .Should().NotThrow();
            
            _beforeUpdate.Verify(before => before.Execute(_ct));
            _update.Verify(update => update.Execute(_ct));
            _afterUpdate.Verify(after => after.Execute(_ct));
        }

        [Fact]
        public void RunCleanupSystems()
        {
            _systemService
                .Awaiting(service => service.Cleanup(_ct))
                .Should().NotThrow();
            
            _cleanup.Verify(cleanup => cleanup.Execute(_ct));
        }
    }
}