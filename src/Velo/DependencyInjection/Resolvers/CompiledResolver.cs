using System;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engines;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class CompiledResolver : DependencyResolver
    {
        private Func<DependencyProvider, object> _builder;
        private ConstructorInfo _constructor;

        public CompiledResolver(Type implementation, DependencyLifetime lifetime)
            : base(implementation, lifetime)
        {
            CheckCanBeActivated(implementation);

            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        public override object Resolve(DependencyProvider scope)
        {
            return _builder(scope);
        }

        protected override void Initialize(DependencyEngine engine)
        {
            if (_builder != null) return;

            var argument = Expression.Parameter(typeof(DependencyProvider));
            var parameters = _constructor.GetParameters();
            var instances = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                var parameterType = parameter.ParameterType;
                var required = !parameter.HasDefaultValue;

                var parameterDependency = engine.GetDependency(parameterType, required);
                var call = parameterDependency == null
                    ? (Expression) Expression.Default(parameterType)
                    : Expression.Call(Expression.Constant(parameterDependency), Dependency.GetInstanceMethod, argument);

                instances[i] = Expression.Convert(call, parameterType);
            }

            Expression body = Expression.New(_constructor, instances);

            _builder = Expression.Lambda<Func<DependencyProvider, object>>(body, argument).Compile();
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
            _builder = null;
            _constructor = null;
        }
    }
}