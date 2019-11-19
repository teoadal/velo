using System;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory
    {
        private class EmptyArrayResolver<T> : DependencyResolver
        {
            protected override object GetInstance(Type contract, IDependencyScope scope)
            {
                return Array.Empty<T>();
            }
        }
    }
}