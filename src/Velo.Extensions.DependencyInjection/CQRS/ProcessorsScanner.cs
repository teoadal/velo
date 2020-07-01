using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS
{
    internal sealed class ProcessorsScanner : AssemblyScanner
    {
        private readonly ServiceLifetime _lifetime;
        private readonly IServiceCollection _services;

        private readonly Action<IServiceCollection, Type, ServiceLifetime> _commandUpsert;
        private readonly Action<IServiceCollection, Type, ServiceLifetime> _notificationUpsert;
        private readonly Action<IServiceCollection, Type, ServiceLifetime> _queryUpsert;
        
        public ProcessorsScanner(IServiceCollection services, ServiceLifetime lifetime)
        {
            _lifetime = lifetime;
            _services = services;

            _commandUpsert = CommandInstaller.UpsertPipeline;
            _notificationUpsert = NotificationInstaller.UpsertPipeline;
            _queryUpsert = QueryInstaller.UpsertPipeline;
        }

        protected override void Handle(Type implementation)
        {
            if (TryRegister(implementation, Types.CommandBehaviourTypes, _commandUpsert)) return;
            if (TryRegister(implementation, Types.CommandProcessorTypes, _commandUpsert)) return;
            if (TryRegister(implementation, Types.NotificationProcessorTypes, _notificationUpsert)) return;
            if (TryRegister(implementation, Types.QueryProcessorTypes, _queryUpsert)) return;
            TryRegister(implementation, Types.QueryBehaviourTypes, _queryUpsert);
        }

        private bool TryRegister(
            Type implementation, 
            Type[] processorTypes,
            Action<IServiceCollection, Type, ServiceLifetime> pipelineHandler)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, processorTypes);

            if (contracts.Length == 0) return false;

            foreach (var contract in contracts)
            {
                _services.Add(new ServiceDescriptor(contract, implementation, _lifetime));
                pipelineHandler(_services, contract, _lifetime);
            }

            return true;
        }
    }
}