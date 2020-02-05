using System;
using System.Diagnostics;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    [DebuggerDisplay("Contract = {_genericContract} Implementation = {_genericImplementation}")]
    internal sealed class GenericFactory : IDependencyFactory
    {
        private readonly Type _genericContract;
        private readonly Type _genericImplementation;
        private readonly DependencyLifetime _lifetime;

        public GenericFactory(Type genericContract, Type genericImplementation, DependencyLifetime lifetime)
        {
            _genericContract = genericContract;
            _genericImplementation = genericImplementation;
            _lifetime = lifetime;
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _genericContract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var implementation = _genericImplementation == null
                ? contract
                : _genericImplementation.MakeGenericType(contract.GenericTypeArguments);

            var resolver = DependencyResolver.Build(_lifetime, implementation, engine);
            return Dependency.Build(_lifetime, new[] {contract}, resolver);
        }
    }
}