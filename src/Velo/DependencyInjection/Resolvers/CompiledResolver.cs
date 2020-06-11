using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Velo.DependencyInjection.Dependencies;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    [DebuggerDisplay("Implementation = {_constructor.DeclaringType}")]
    internal sealed class CompiledResolver : DependencyResolver
    {
        private static readonly MethodInfo GetInstanceMethod =
            typeof(IDependency).GetMethod(nameof(IDependency.GetInstance))!;

        private Func<IServiceProvider, object>? _builder;
        private readonly ConstructorInfo _constructor;
        private IDependencyEngine _dependencyEngine;

        public CompiledResolver(Type implementation)
            : base(implementation)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation)
                           ?? throw Error.DefaultConstructorNotFound(implementation);

            _dependencyEngine = null!;
        }

        public override void Init(DependencyLifetime lifetime, IDependencyEngine engine)
        {
            EnsureValidDependenciesLifetime(_constructor, lifetime, engine);

            _dependencyEngine = engine;
        }

        protected override object ResolveInstance(Type contract, IServiceProvider services)
        {
            _builder ??= CreateBuilder(services);
            return _builder(services);
        }

        private Func<IServiceProvider, object> CreateBuilder(IServiceProvider services)
        {
            var servicesArgument = Expression.Parameter(typeof(IServiceProvider), "services");

            var constructorParameters = _constructor.GetParameters();
            var parameters = new Expression[constructorParameters.Length];
            for (var i = constructorParameters.Length - 1; i >= 0; i--)
            {
                var parameter = constructorParameters[i];
                var parameterType = parameter.ParameterType;
                var required = !parameter.HasDefaultValue;

                var parameterDependency = required
                    ? _dependencyEngine.GetRequiredDependency(parameterType)
                    : _dependencyEngine.GetDependency(parameterType);

                parameters[i] = parameterDependency == null
                    ? Expression.Default(parameterType)
                    : BuildParameter(parameterDependency, parameterType, services, servicesArgument);
            }

            var body = Expression.New(_constructor, parameters);
            return Expression.Lambda<Func<IServiceProvider, object>>(body, servicesArgument).Compile();
        }

        private static Expression BuildParameter(
            IDependency parameterDependency,
            Type parameterType,
            IServiceProvider services,
            Expression servicesArgument)
        {
            if (parameterDependency.Lifetime == DependencyLifetime.Singleton)
            {
                var instance = parameterDependency.GetInstance(parameterType, services);
                return Expression.Constant(instance);
            }

            var dependency = Expression.Constant(parameterDependency);

            var parameterCall = Expression.Call(dependency, GetInstanceMethod,
                Expression.Constant(parameterType),
                servicesArgument);

            return Expression.Convert(parameterCall, parameterType);
        }
    }
}