using System;
using System.Reflection;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    public abstract class Dependency : IDisposable
    {
        internal static readonly MethodInfo GetInstanceMethod = typeof(Dependency).GetMethod(nameof(GetInstance));
        
        public readonly DependencyResolver Resolver;

        protected Dependency(DependencyResolver resolver)
        {
            Resolver = resolver;
        }

        public abstract object GetInstance(DependencyProvider scope);

        public virtual void Dispose()
        {
            Resolver.Dispose();
        }
    }
}