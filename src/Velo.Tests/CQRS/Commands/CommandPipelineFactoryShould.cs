using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Commands
{
    public class CommandPipelineFactoryShould : CQRSTestClass
    {
        private readonly Mock<IDependencyEngine> _dependencyEngine;
        private readonly CommandPipelineFactory _factory;
        private readonly Type _pipelineType;
        
        private DependencyLifetime _processorsLifetime;

        public CommandPipelineFactoryShould(ITestOutputHelper output) : base(output)
        {
            _dependencyEngine = new Mock<IDependencyEngine>();
            _dependencyEngine
                .Setup(e => e.GetDependency(It.IsNotNull<Type>(), true))
                .Returns<Type, bool>((type, _) =>
                {
                    var dependency = new Mock<IDependency>();
                    dependency.SetupGet(d => d.Contracts).Returns(new[] {type});
                    dependency.SetupGet(d => d.Lifetime).Returns(_processorsLifetime);
                    return dependency.Object;
                });
            
            _factory = new CommandPipelineFactory();
            _pipelineType = typeof(ICommandPipeline<Command>);
        }
        
        [Fact]
        public void CreateDependency()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            dependency.Contracts.Should().Contain(_pipelineType);
            dependency.Resolver.Implementation.Should().Be(typeof(CommandPipeline<Command>));
        }
        
        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            _processorsLifetime = lifetime;
            
            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }
        
        [Fact]
        public void CallDependencyEngine()
        {
            _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandBehaviour<Command>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandPreProcessor<Command>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandProcessor<Command>), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandPostProcessor<Command>[]), true));
        }
    }
}