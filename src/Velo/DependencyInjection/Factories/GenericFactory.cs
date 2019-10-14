using System;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed class GenericFactory : ResolverFactory
    {
        private readonly DependencyLifetime _lifetime;
        private readonly Type _genericContract;
        private readonly Type _genericImplementation;

        public GenericFactory(Type genericContract, Type genericImplementation, DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
            _genericContract = genericContract;
            _genericImplementation = genericImplementation;
            
            CheckIsGenericTypeDefinition(genericContract);
            CheckIsGenericTypeDefinition(genericImplementation);
        }

        public override bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _genericContract);
        }

        protected override DependencyResolver BuildResolver(Type contract)
        {
            var implementation = _genericImplementation == null
                ? contract
                : _genericImplementation.MakeGenericType(contract.GenericTypeArguments);

            var resolver = _lifetime == DependencyLifetime.Singleton
                ? (DependencyResolver) new ActivatorResolver(implementation, DependencyLifetime.Singleton)
                : new CompiledResolver(implementation, _lifetime);

            return resolver;
        }

        private static void CheckIsGenericTypeDefinition(Type type)
        {
            if (type != null && !type.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"{ReflectionUtils.GetName(type)} is not generic type definition");
            }
        }
    }
}