using System;
using Velo.DependencyInjection.Engines;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    public abstract class DependencyResolver : IDisposable
    {
        public readonly Type Implementation;
        
        public readonly DependencyLifetime Lifetime;

        private bool _circularGuard;
        private bool _initialized;
        
        protected DependencyResolver(Type implementation, DependencyLifetime lifetime)
        {
            Implementation = implementation;
            Lifetime = lifetime;
        }

        public void Init(DependencyEngine engine)
        {
            if (_initialized) return;

            if (_circularGuard)
            {
                throw Error.CircularDependency(Implementation);
            }

            _circularGuard = true;
            
            Initialize(engine);
            
            _circularGuard = false;
            _initialized = true;
        }

        public abstract object Resolve(DependencyProvider scope);

        protected abstract void Initialize(DependencyEngine engine);

        public abstract void Dispose();
    }
}