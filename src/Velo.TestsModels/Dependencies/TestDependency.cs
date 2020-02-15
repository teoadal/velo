using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;

namespace Velo.TestsModels.Dependencies
{
    public sealed class TestDependency : Dependency
    {
        public TestDependency(DependencyResolver resolver)
            : base(Array.Empty<Type>(), resolver, DependencyLifetime.Singleton)
        {
        }
        
        public TestDependency(DependencyLifetime lifetime)
            : this(Array.Empty<Type>(), lifetime)
        {
        }

        public TestDependency(Type[] contracts, DependencyLifetime lifetime = DependencyLifetime.Singleton)
            : base(contracts, new InstanceResolver(new object()), lifetime)
        {
        }

        public override object GetInstance(Type contract, IDependencyScope scope)
        {
            return null;
        }

        public override void Dispose()
        {
        }
    }
}