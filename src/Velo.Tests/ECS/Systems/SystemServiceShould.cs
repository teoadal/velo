using FluentAssertions;
using Moq;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Pipelines;
using Xunit;

namespace Velo.Tests.ECS.Systems
{
    public class SystemServiceShould : ECSTestClass
    {
        private readonly Mock<ISystemPipeline<IBootstrapSystem>> _bootstrap;
        private readonly Mock<ISystemPipeline<IInitSystem>> _init;
        private readonly Mock<ISystemPipeline<IBeforeUpdateSystem>> _beforeUpdate;
        private readonly Mock<ISystemPipeline<IUpdateSystem>> _update;
        private readonly Mock<ISystemPipeline<IAfterUpdateSystem>> _afterUpdate;
        private readonly Mock<ISystemPipeline<ICleanupSystem>> _cleanup;

        private readonly ISystemService _systemService;

        public SystemServiceShould()
        {
            _bootstrap = new Mock<ISystemPipeline<IBootstrapSystem>>();
            _init = new Mock<ISystemPipeline<IInitSystem>>();
            _beforeUpdate = new Mock<ISystemPipeline<IBeforeUpdateSystem>>();
            _update = new Mock<ISystemPipeline<IUpdateSystem>>();
            _afterUpdate = new Mock<ISystemPipeline<IAfterUpdateSystem>>();
            _cleanup = new Mock<ISystemPipeline<ICleanupSystem>>();

            _systemService = new SystemService(
                _bootstrap.Object,
                _init.Object,
                _beforeUpdate.Object,
                _update.Object,
                _afterUpdate.Object,
                _cleanup.Object);
        }

        [Fact]
        public void RunBootstrapSystems()
        {
            _systemService
                .Awaiting(service => service.Bootstrap(CancellationToken))
                .Should().NotThrow();
            
            _bootstrap.Verify(bootstrap => bootstrap.Execute(CancellationToken));
        }
        
        [Fact]
        public void RunInitSystems()
        {
            _systemService
                .Awaiting(service => service.Init(CancellationToken))
                .Should().NotThrow();
            
            _init.Verify(init => init.Execute(CancellationToken));
        }

        [Fact]
        public void RunUpdateSystems()
        {
            _systemService
                .Awaiting(service => service.Update(CancellationToken))
                .Should().NotThrow();
            
            _beforeUpdate.Verify(before => before.Execute(CancellationToken));
            _update.Verify(update => update.Execute(CancellationToken));
            _afterUpdate.Verify(after => after.Execute(CancellationToken));
        }

        [Fact]
        public void RunCleanupSystems()
        {
            _systemService
                .Awaiting(service => service.Cleanup(CancellationToken))
                .Should().NotThrow();
            
            _cleanup.Verify(cleanup => cleanup.Execute(CancellationToken));
        }
    }
}