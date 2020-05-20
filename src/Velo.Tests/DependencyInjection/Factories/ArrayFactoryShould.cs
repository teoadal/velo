using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Factories
{
    public class ArrayFactoryShould : DITestClass
    {
        private readonly Mock<IDependencyEngine> _engine;

        private readonly Type _arrayType;
        private readonly Type _elementType;
        private readonly Type _enumerableType;
        private readonly IServiceProvider _services;

        private readonly ArrayFactory _factory;

        public ArrayFactoryShould()
        {
            _engine = new Mock<IDependencyEngine>();

            _elementType = typeof(Boo);
            _arrayType = _elementType.MakeArrayType();
            _enumerableType = typeof(IEnumerable<>).MakeGenericType(_elementType);
            _services = Mock.Of<IServiceProvider>();

            _factory = new ArrayFactory();

            SetupApplicableDependencies(_engine, _elementType);
        }

        [Fact]
        public void ApplicableForArray()
        {
            _factory.Applicable(_arrayType).Should().BeTrue();
        }

        [Fact]
        public void ApplicableForEnumerable()
        {
            _factory.Applicable(_enumerableType).Should().BeTrue();
        }

        [Fact]
        public void BuildArrayDependency()
        {
            var dependency = Act(_enumerableType);
            dependency.Implementation.Should().Be(_arrayType);
        }

        [Fact]
        public void BuildEmptyArrayDependency()
        {
            _engine.Reset();

            var dependency = Act(_enumerableType);
            dependency.Implementation.Should().Be(_arrayType);
        }

        [Fact]
        public void BuildEnumerableDependency()
        {
            var dependency = Act(_enumerableType);
            dependency.Implementation.Should().Be(_arrayType);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void BuildWithValidLifetime(DependencyLifetime lifetime)
        {
            _engine.Reset();
            
            SetupApplicableDependencies(_engine, _elementType, lifetime);
            
            var dependency = Act(_enumerableType);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CallDependencyEngine()
        {
            Act(_arrayType);

            _engine.Verify(engine => engine.GetApplicable(_elementType));
        }

        #region Resolver

        [Fact]
        public void Resolver_CallDependencies()
        {
            _engine.Reset();

            var dependencies = SetupApplicableDependencies(_engine, _elementType);
            
            Act(_arrayType).GetInstance(_arrayType, _services);
            
            foreach (var dependency in dependencies)
            {
                dependency.Verify(d => d.GetInstance(_elementType, _services));
            }
        }
        
        [Fact]
        public void Resolver_ResolveArray()
        {
            var dependency = Act(_arrayType);
            var instance = dependency.GetInstance(_arrayType, _services);

            instance.Should().BeOfType<Boo[]>();
        }

        [Fact]
        public void Resolver_ResolveEmptyArray()
        {
            _engine.Reset();

            var dependency = Act(_arrayType);
            var instance = (Boo[]) dependency.GetInstance(_arrayType, _services);

            instance.Should().BeEmpty();
        }

        [Fact]
        public void Resolver_ResolveEnumerable()
        {
            var dependency = Act(_arrayType);
            var instance = (IEnumerable<Boo>) dependency.GetInstance(_arrayType, _services);

            instance.Should().NotBeEmpty();
        }

        #endregion

        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(_elementType).Should().BeFalse();
        }

        private IDependency Act(Type collectionType)
        {
            return _factory
                .Invoking(factory => factory.BuildDependency(collectionType, _engine.Object))
                .Should().NotThrow().Subject;
        }
    }
}