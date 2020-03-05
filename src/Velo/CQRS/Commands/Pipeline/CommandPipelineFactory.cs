using System;
using Velo.Collections;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS.Commands.Pipeline
{
    public class CommandPipelineFactory : IDependencyFactory
    {
        private readonly Type _contract;
        private readonly Type _fullPipelineImplementation;

        public CommandPipelineFactory()
        {
            _contract = typeof(ICommandPipeline<>);
            _fullPipelineImplementation = typeof(CommandPipeline<>);
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var implementation = _fullPipelineImplementation.MakeGenericType(contract.GenericTypeArguments);
            var constructor = ReflectionUtils.GetConstructor(implementation);
            var parameters = constructor.GetParameters();

            var dependencies = new LocalList<IDependency>();
            foreach (var parameter in parameters)
            {
                var required = !parameter.HasDefaultValue;
                var dependency = engine.GetDependency(parameter.ParameterType, required);
                dependencies.Add(dependency);
            }
            
            var lifetime = dependencies.DefineLifetime();
            var resolver = DependencyResolver.Build(lifetime, implementation, engine);

            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }
    }
}