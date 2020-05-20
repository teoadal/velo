using System;
using System.Diagnostics;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory
    {
        [DebuggerDisplay("Empty array of {typeof(T).Name}")]
        private sealed class EmptyArrayResolver<T> : DependencyResolver
        {
            public EmptyArrayResolver() : base(Typeof<T[]>.Raw)
            {
            }
            
            protected override object ResolveInstance(Type contract, IServiceProvider services)
            {
                return Array.Empty<T>();
            }
        }
    }
}