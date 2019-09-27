using System;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Dependencies.Resolvers;
using Velo.Utils;

namespace Velo.Dependencies.Transients
{
    internal sealed class CompiledTransient : Dependency
    {
        private static readonly Type ResolverType = typeof(IDependencyResolver);
        private static readonly MethodInfo ResolveMethod = ResolverType.GetMethod(nameof(IDependencyResolver.Resolve));

        private Func<DependencyContainer, object> _builder;
        private readonly ConstructorInfo _constructor;

        public CompiledTransient(Type[] contracts, Type implementation) : base(contracts)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            if (_builder == null) _builder = CreateBuilder(_constructor, container);
            return _builder(container);
        }

        private static Func<DependencyContainer, object> CreateBuilder(ConstructorInfo constructor,
            DependencyContainer container)
        {
            var containerParameter = Expression.Parameter(typeof(DependencyContainer), "container");

            var parameters = constructor.GetParameters();
            var resolvedParameters = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                var parameterType = parameter.ParameterType;
                var parameterName = parameter.Name;
                var required = !parameter.HasDefaultValue;

                var parameterResolver = container.GetResolver(parameterType, parameterName, required);
                var resolvedParameter = parameterResolver == null
                    ? (Expression) Expression.Default(parameterType)
                    : Expression.Call(Expression.Constant(parameterResolver), ResolveMethod,
                        Expression.Constant(parameterType), containerParameter);

                resolvedParameters[i] = Expression.Convert(resolvedParameter, parameterType);
            }

            var body = Expression.New(constructor, resolvedParameters);
            return Expression.Lambda<Func<DependencyContainer, object>>(body, containerParameter).Compile();
        }
    }
}