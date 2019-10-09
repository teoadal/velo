using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    [DebuggerDisplay("{" + nameof(_dependency) + "}")]
    internal sealed class DefaultResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly object _lockObject;
        private bool _resolveInProgress;
        
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BeginResolving()
        {
            if (_resolveInProgress) throw Error.CircularDependency(_dependency);
            _resolveInProgress = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResolvingComplete()
        {
            _resolveInProgress = false;
        }
    }
}