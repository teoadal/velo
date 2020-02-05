using System;
using Velo.Collections;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS.Pipeline
{
    internal sealed class PipelineFactory : IDependencyFactory
    {
        private readonly Type _pipelineType;

        public PipelineFactory(Type pipelineType)
        {
            _pipelineType = pipelineType;
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _pipelineType);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var constructor = ReflectionUtils.GetConstructor(contract);
            var parameters = constructor.GetParameters();

            var dependencies = new LocalList<IDependency>();
            foreach (var parameter in parameters)
            {
                var dependency = engine.GetDependency(parameter.ParameterType);
                dependencies.Add(dependency);
            }

            var lifetime = dependencies.DefineLifetime();
            var resolver = DependencyResolver.Build(lifetime, contract, engine);
            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }
    }
}