using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemHandlerFactoryShould : ECSTestClass
    {
        private readonly Mock<IDependencyEngine> _engine;
        private readonly Type _handlerType;
        private readonly Type _systemType;
        
        private readonly SystemHandlerFactory _factory;
        
        public SystemHandlerFactoryShould(ITestOutputHelper output) : base(output)
        {
            _engine = new Mock<IDependencyEngine>();
            _systemType = typeof(IUpdateSystem);
            _handlerType = typeof(ISystemHandler<>).MakeGenericType(_systemType);
            
            _factory = new SystemHandlerFactory();
        }

        [Theory]
        [MemberData(nameof(SystemHandlerTypes))]
        public void Applicable(Type contract)
        {
            _factory.Applicable(contract).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            _engine.SetupApplicable(_systemType, TestUtils.MockDependency(lifetime).Object);
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CreateFullWithSystemsMix()
        {
            var parallelSystemType = typeof(ParallelSystem);

            _engine.SetupApplicable(_systemType, Many(() => TestUtils
                    .MockDependency(DependencyLifetime.Singleton, parallelSystemType)
                    .SetupResolverImplementation(parallelSystemType)
                    .Object)
                .Append(TestUtils
                    .MockDependency(DependencyLifetime.Singleton, _systemType)
                    .SetupResolverImplementation(_systemType).Object)
                .ToArray());
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be(typeof(SystemFullHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateNullWithoutSystems()
        {
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be(typeof(SystemNullHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateSingleWithOneSystem()
        {
            _engine.SetupApplicable(_systemType, Mock.Of<IDependency>());
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be(typeof(SystemSingleHandler<>).MakeGenericType(_systemType));
        }

        [Fact]
        public void CreateParallelWithParallelSystems()
        {
            var parallelSystemType = typeof(ParallelSystem);

            _engine.SetupApplicable(_systemType, Many(() => TestUtils
                .MockDependency(DependencyLifetime.Singleton, parallelSystemType)
                .SetupResolverImplementation(parallelSystemType)
                .Object));
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be(typeof(SystemParallelHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateSequentialWithoutParallelSystems()
        {
            _engine.SetupApplicable(_systemType, Many(() => TestUtils
                .MockDependency(DependencyLifetime.Singleton, _systemType)
                .SetupResolverImplementation(_systemType)
                .Object));
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be(typeof(SystemSequentialHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(object)).Should().BeFalse();
        }
        
        public static TheoryData<Type> SystemHandlerTypes = new TheoryData<Type>
        {
            typeof(ISystemHandler<IInitSystem>),
            
            typeof(ISystemHandler<IBeforeUpdateSystem>),
            typeof(ISystemHandler<IUpdateSystem>),
            typeof(ISystemHandler<IAfterUpdateSystem>),
            
            typeof(ISystemHandler<ICleanupSystem>)
        };
    }
    
    
}