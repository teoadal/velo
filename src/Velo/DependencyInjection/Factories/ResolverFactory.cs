using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Factories
{
    public abstract class ResolverFactory
    {
        private readonly Dictionary<Type, DependencyResolver> _existsResolvers;

        protected ResolverFactory()
        {
            _existsResolvers = new Dictionary<Type, DependencyResolver>(2);
        }

        public abstract bool Applicable(Type contract);

        public DependencyResolver GetResolver(Type contract)
        {
            if (_existsResolvers.TryGetValue(contract, out var existsResolver))
            {
                return existsResolver;
            }

            var resolver = BuildResolver(contract);
            
            _existsResolvers.Add(contract, resolver);
            
            return resolver;
        }

        protected abstract DependencyResolver BuildResolver(Type contract);
    }
}