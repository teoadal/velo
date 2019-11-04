using System;
using System.Reflection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engines;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class ActivatorResolver : DependencyResolver
    {
        private ConstructorInfo _constructor;
        private Dependency[] _dependencies;

        public ActivatorResolver(Type implementation, DependencyLifetime lifetime)
            : base(implementation, lifetime)
        {
            CheckCanBeActivated(implementation);

            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        public override object Resolve(DependencyProvider scope)
        {
            var dependencies = _dependencies;
            var parameters = new object[dependencies.Length];
            
            for (var i = 0; i < dependencies.Length; i++)
            {
                parameters[i] = dependencies[i]?.GetInstance(scope);
            }

            return _constructor.Invoke(parameters);
        }

        protected override void Initialize(DependencyEngine engine)
        {
            var parameters = _constructor.GetParameters();
            var dependencies = new Dependency[parameters.Length];
            
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var required = !parameter.HasDefaultValue;

                dependencies[i] = engine.GetDependency(parameter.ParameterType, required);
            }

            _dependencies = dependencies;
        }

        private static void CheckCanBeActivated(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                throw Error.InvalidOperation($"{ReflectionUtils.GetName(type)} type can't be activated");
            }
        }
        
        public override void Dispose()
        {
            _constructor = null;
            _dependencies = null;
        }
    }
}