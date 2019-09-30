using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class DefaultResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly object _lockObject;
        
        public DefaultResolver(IDependency dependency): base(dependency)
        {
            _dependency = dependency;
            _lockObject = new object();
        }
        
        public override object Resolve(Type contract, DependencyContainer container)
        {
            using (Lock.Enter(_lockObject))
            {
                BeginResolving();
                
                var resolved = _dependency.Resolve(contract, container);
                
                ResolvingComplete();

                return resolved;
            }
        }
    }
}