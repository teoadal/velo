using System;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class EmitterAllover : DependencyAllover
    {
        private readonly DependencyLifetime _lifetime;
        private readonly Type _commandHandlerContract;
        private readonly Type _queryHandlerContract;

        public EmitterAllover(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
            _commandHandlerContract = typeof(ICommandHandler);
            _queryHandlerContract = typeof(IQueryHandler);
        }

        public override void TryRegister(DependencyCollection collection, Type implementation)
        {
            Type contract;
            if (_commandHandlerContract.IsAssignableFrom(implementation))
            {
                contract = _commandHandlerContract;
            }
            else if (_queryHandlerContract.IsAssignableFrom(implementation))
            {
                contract = _queryHandlerContract;
            }
            else return;

            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, contract);
            collection.Add(contracts.ToArray(), implementation, _lifetime);
        }
    }
}