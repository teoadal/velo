using System;
using FluentAssertions;
using Velo.CQRS.Commands;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Dependencies;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Emitting;
using Velo.TestsModels.Foos;
using Xunit;

namespace Velo.DependencyInjection
{
    public class DependencyShould
    {
        private static readonly Type[] ClassContracts = {typeof(Boo), typeof(Foo)};
        
        [Fact]
        public void BeApplicableByClass()
        {
            var dependency = new SingletonDependency(ClassContracts, new object());

            foreach (var contract in ClassContracts)
            {
                dependency.Applicable(contract).Should().BeTrue();
            }
        }
        
        [Fact]
        public void BeApplicableByInterface()
        {
            var contracts = new[] {
                typeof(MeasureBehaviour), 
                typeof(ICommandBehaviour<IMeasureCommand>)};
            
            var dependency = new SingletonDependency(contracts, new object());

            foreach (var contract in contracts)
            {
                dependency.Applicable(contract).Should().BeTrue();
            }
        }
        
        [Fact]
        public void BeApplicableByContravariantInterface()
        {
            var contracts = new[] {typeof(ICommandBehaviour<IMeasureCommand>)};
            var dependency = new SingletonDependency(contracts, new object());

            var contravariantInterface = typeof(ICommandBehaviour<TestsModels.Emitting.Boos.Create.Command>);

            dependency.Applicable(contravariantInterface).Should().BeTrue();
        }
        
        [Fact]
        public void HasValidContracts()
        {
            var dependency = new TestDependency(ClassContracts);
            dependency.Contracts.Should().Contain(ClassContracts);
        }
        
        [Fact]
        public void HasValidLifetime()
        {
            var lifetimes = (DependencyLifetime[]) Enum.GetValues(typeof(DependencyLifetime));
            foreach (var lifetime in lifetimes)
            {
                var dependency = new TestDependency(lifetime);
                dependency.Lifetime.Should().Be(lifetime);
            }
        }
        
        [Fact]
        public void HasValidResolver()
        {
            var instance = new object();
            var dependencyResolver = new InstanceResolver(instance);
            
            var dependency = new TestDependency(dependencyResolver);
            var resolver = dependency.Resolver;
            
            resolver.Should().Be(dependencyResolver);
            resolver.Implementation.Should().Be(instance.GetType());
        }

        [Fact]
        public void Resolve()
        {
            var instance = new object();
            var dependency = new SingletonDependency(ClassContracts, instance);

            dependency.GetInstance(null, null).Should().Be(instance);
        }
        
        [Fact]
        public void NotBeApplicableByClass()
        {
            var dependency = new SingletonDependency(ClassContracts, new object());
            dependency.Applicable(typeof(object)).Should().BeFalse();
        }
        
        [Fact]
        public void NotBeApplicableByInterface()
        {
            var contracts = new[] {typeof(IRepository)};
            var dependency = new SingletonDependency(contracts, new object());
            dependency.Applicable(typeof(IRepository<Boo>)).Should().BeFalse();
        }
        
        [Fact]
        public void NotBeApplicableByContravariantInterface()
        {
            var contracts = new[] {typeof(ICommandBehaviour<IMeasureCommand>)};
            var dependency = new SingletonDependency(contracts, new object());

            var contravariantInterface = typeof(ICommandBehaviour<ICommand>);

            dependency.Applicable(contravariantInterface).Should().BeFalse();
        }
    }
}