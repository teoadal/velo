using System;

namespace Velo.DependencyInjection.Dependencies
{
    public interface IDependency : IDisposable
    {
        Type[] Contracts { get; }
        
        DependencyLifetime Lifetime { get; }
        
        bool Applicable(Type contract);

        object GetInstance(Type contract, IDependencyScope scope);
    }
    
    public abstract class Dependency : IDependency
    {
        public Type[] Contracts => _contracts;
        
        public DependencyLifetime Lifetime { get; }

        private Type[] _contracts;
        
        protected Dependency(Type[] contracts, DependencyLifetime lifetime)
        {
            Lifetime = lifetime;
            _contracts = contracts;
        }

        public bool Applicable(Type request)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var contract in _contracts)
            {
                if (contract.IsAssignableFrom(request)) return true;
            }

            return false;
        }

        public abstract object GetInstance(Type contract, IDependencyScope scope);

        public abstract void Dispose();
    }
}