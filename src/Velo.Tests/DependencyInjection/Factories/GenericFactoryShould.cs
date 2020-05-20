using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Factories
{
    public class GenericFactoryShould : DITestClass
    {
        private readonly Type _genericContract;
        private readonly Type _genericImplementation;

        private readonly IDependencyEngine _engine;

        private readonly GenericFactory _factory;

        public GenericFactoryShould()
        {
            _genericContract = typeof(IList<>);
            _genericImplementation = typeof(List<>);

            _engine = Mock.Of<IDependencyEngine>();

            _factory = new GenericFactory(_genericContract, _genericImplementation, DependencyLifetime.Singleton);
        }

        [Theory, MemberData(nameof(Types))]
        public void Applicable(Type type)
        {
            var contract = _genericContract.MakeGenericType(type);
            _factory.Applicable(contract).Should().BeTrue();
        }

        [Theory, MemberData(nameof(Types))]
        public void BuildDependency(Type type)
        {
            var contract = _genericContract.MakeGenericType(type);
            
            var dependency = _factory.BuildDependency(contract, _engine);
            dependency.Implementation.Should().Be(_genericImplementation.MakeGenericType(type));
        }
        
        [Theory, MemberData(nameof(Lifetimes))]
        public void BuildWithValidLifetime(DependencyLifetime lifetime)
        {
            var factory = new GenericFactory(_genericContract, _genericImplementation, lifetime);
            
            var dependency = factory.BuildDependency(_genericContract.MakeGenericType(typeof(int)), _engine);
            dependency.Lifetime.Should().Be(lifetime);
        }
        
        [Theory, MemberData(nameof(Types))]
        public void BuildWithoutImplementation(Type type)
        {
            var factory = new GenericFactory(_genericImplementation, null, DependencyLifetime.Singleton);
         
            var contract = _genericImplementation.MakeGenericType(type);

            factory.Applicable(contract).Should().BeTrue();
            
            var dependency = factory.BuildDependency(contract, _engine);
            dependency.Implementation.Should().Be(contract);
        }
        
        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(Boo)).Should().BeFalse();
        }

        public static TheoryData<Type> Types => new TheoryData<Type>
        {
            typeof(string),
            typeof(int),
            typeof(double?),
            typeof(Boo[]),
            typeof(Boo)
        };
    }
}