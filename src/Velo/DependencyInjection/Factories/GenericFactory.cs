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

            switch (_lifetime)
            {
                case DependencyLifetime.Scope:
                    return new ScopeDependency(contract, new CompiledResolver(implementation, engine));
                case DependencyLifetime.Singleton:
                    return new SingletonDependency(contract, new ActivatorResolver(implementation));
                case DependencyLifetime.Transient:
                    return new TransientDependency(contract, new CompiledResolver(implementation, engine));
                default:
                    throw Error.InvalidDependencyLifetime();
            }
        }
    }
}