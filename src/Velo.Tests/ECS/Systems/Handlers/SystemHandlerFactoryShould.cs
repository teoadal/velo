using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.ECS.Systems;
using Velo.ECS.Systems.Handlers;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Systems.Handlers
{
    public class SystemHandlerFactoryShould : ECSTestClass
    {
        private readonly Mock<IDependencyEngine> _engine;
        private readonly Type _handlerType;
        private readonly Type _systemType;
        
        private readonly SystemHandlerFactory _factory;
        
        public SystemHandlerFactoryShould()
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
            SetupApplicable(_engine, _systemType, MockDependency(lifetime).Object);
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CreateFullWithSystemsMix()
        {
            var parallelSystemType = typeof(ParallelSystem);

            SetupApplicable(_engine, _systemType, Many(() => MockDependency(DependencyLifetime.Singleton, parallelSystemType).Object)
                .Append(MockDependency(DependencyLifetime.Singleton, _systemType).Object)
                .ToArray());
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Implementation.Should().Be(typeof(SystemFullHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateNullWithoutSystems()
        {
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Implementation.Should().Be(typeof(SystemNullHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateSingleWithOneSystem()
        {
            SetupApplicable(_engine, _systemType, Mock.Of<IDependency>());
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Implementation.Should().Be(typeof(SystemSingleHandler<>).MakeGenericType(_systemType));
        }

        [Fact]
        public void CreateParallelWithParallelSystems()
        {
            var parallelSystemType = typeof(ParallelSystem);

            SetupApplicable(_engine, _systemType, Many(() => MockDependency(DependencyLifetime.Singleton, parallelSystemType).Object));
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Implementation.Should().Be(typeof(SystemParallelHandler<>).MakeGenericType(_systemType));
        }
        
        [Fact]
        public void CreateSequentialWithoutParallelSystems()
        {
            SetupApplicable(_engine, _systemType, Many(() => MockDependency(DependencyLifetime.Singleton, _systemType).Object));
            
            var dependency = _factory.BuildDependency(_handlerType, _engine.Object);
            dependency.Implementation.Should().Be(typeof(SystemSequentialHandler<>).MakeGenericType(_systemType));
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